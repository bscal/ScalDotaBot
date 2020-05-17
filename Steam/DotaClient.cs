using System;
using System.Collections.Generic;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SteamKit2.GC;
using SteamKit2.Internal;
using SteamKit2.GC.Dota.Internal;

namespace InhouseBot.Steam
{
    enum DotaGameResult
    {
        Radiant,
        Dire,
        Unknown
    }


    public class DotaClient
    {
        SteamClient steamClient;
        CallbackManager manager;

        SteamUser steamUser;
        SteamGameCoordinator gameCoordinator;

        public bool isRunning;

        string user, pass;

        // dota2's appid
        const int APPID = 570;

        public void Start(string[] args)
        {

            DebugLog.AddListener((category, msg) => Console.WriteLine("AnonymousMethod - {0}: {1}", category, msg));
            DebugLog.Enabled = false;

            if (args.Length < 2)
            {
                Console.WriteLine("Sample1: No username and password specified!");
                return;
            }

            // save our logon details
            user = args[0];
            pass = args[1];

            // create our steamclient instance
            steamClient = new SteamClient();
            // create the callback manager which will route callbacks to function calls
            manager = new CallbackManager(steamClient);

            // get the steamuser handler, which is used for logging on after successfully connecting
            steamUser = steamClient.GetHandler<SteamUser>();

            gameCoordinator = steamClient.GetHandler<SteamGameCoordinator>();
            manager.Subscribe<SteamGameCoordinator.MessageCallback>(OnGCMessage);
            // register a few callbacks we're interested in
            // these are registered upon creation to a callback manager, which will then route the callbacks
            // to the functions specified
            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);

            isRunning = true;

            Console.WriteLine("Connecting to Steam...");

            // initiate the connection
            steamClient.Connect();

            // create our callback handling loop
            while (isRunning)
            {
                // in order for the callbacks to get routed, they need to be handled by the manager
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }

        // called when a gamecoordinator (GC) message arrives
        // these kinds of messages are designed to be game-specific
        // in this case, we'll be handling dota's GC messages
        void OnGCMessage(SteamGameCoordinator.MessageCallback callback)
        {
            // setup our dispatch table for messages
            // this makes the code cleaner and easier to maintain
            var messageMap = new Dictionary<uint, Action<IPacketGCMsg>>
            {
                { ( uint )EGCBaseClientMsg.k_EMsgGCClientWelcome, OnClientWelcome },
                        //                {(uint) EDOTAGCMsg.k_EMsgGCPracticeLobbyJoinResponse, HandlePracticeLobbyJoinResponse},
                        //{(uint) EDOTAGCMsg.k_EMsgGCPracticeLobbyListResponse, HandlePracticeLobbyListResponse},
                        //{(uint) ESOMsg.k_ESOMsg_UpdateMultiple, HandleUpdateMultiple},
                        //{(uint) ESOMsg.k_ESOMsg_CacheSubscribed, HandleCacheSubscribed},
                        //{(uint) ESOMsg.k_ESOMsg_CacheUnsubscribed, HandleCacheUnsubscribed},
                        //{(uint) ESOMsg.k_ESOMsg_Destroy, HandleCacheDestroy},
                        //{(uint) EGCBaseClientMsg.k_EMsgGCPingRequest, HandlePingRequest},
                        //{(uint) EDOTAGCMsg.k_EMsgGCJoinChatChannelResponse, HandleJoinChatChannelResponse},
                        //{(uint) EDOTAGCMsg.k_EMsgGCRequestChatChannelListResponse, HandleChatChannelListResponse},
                        //{(uint) EDOTAGCMsg.k_EMsgGCChatMessage, HandleChatMessage},
                        //{(uint) EDOTAGCMsg.k_EMsgGCOtherJoinedChannel, HandleOtherJoinedChannel},
                        //{(uint) EDOTAGCMsg.k_EMsgGCOtherLeftChannel, HandleOtherLeftChannel},
                        //{(uint) EDOTAGCMsg.k_EMsgGCPopup, HandlePopup},
                        //{(uint) EDOTAGCMsg.k_EMsgDOTALiveLeagueGameUpdate, HandleLiveLeageGameUpdate},
                        //{(uint) EGCBaseMsg.k_EMsgGCInvitationCreated, HandleInvitationCreated},
                        //{(uint) EDOTAGCMsg.k_EMsgGCMatchDetailsResponse, HandleMatchDetailsResponse},
                        //{(uint) EGCBaseClientMsg.k_EMsgGCClientConnectionStatus, HandleConnectionStatus},
                        //{(uint) EDOTAGCMsg.k_EMsgGCProTeamListResponse, HandleProTeamList},
                        //{(uint) EDOTAGCMsg.k_EMsgGCFantasyLeagueInfo, HandleFantasyLeagueInfo},
                        //{(uint) EDOTAGCMsg.k_EMsgGCPlayerInfo, HandlePlayerInfo},
                        //{(uint) EDOTAGCMsg.k_EMsgGCProfileResponse, HandleProfileResponse},
                        //{(uint) EDOTAGCMsg.k_EMsgGCGuildSetAccountRoleResponse, HandleGuildAccountRoleResponse},
                        //{(uint) EDOTAGCMsg.k_EMsgGCGuildInviteAccountResponse, HandleGuildInviteAccountResponse},
                        //{(uint) EDOTAGCMsg.k_EMsgGCGuildCancelInviteResponse, HandleGuildCancelInviteResponse},
                        //{(uint) EDOTAGCMsg.k_EMsgGCGuildData, HandleGuildData},
                        //{(uint) EDOTAGCMsg.k_EMsgClientToGCGetProfileCardResponse, HandleProfileCardResponse}
            };

            Action<IPacketGCMsg> func;
            if (!messageMap.TryGetValue(callback.EMsg, out func))
            {
                // this will happen when we recieve some GC messages that we're not handling
                // this is okay because we're handling every essential message, and the rest can be ignored
                return;
            }

            func(callback.Message);
        }

        // this happens after telling steam that we launched dota (with the ClientGamesPlayed message)
        // this can also happen after the GC has restarted (due to a crash or new version)
        void OnClientWelcome(IPacketGCMsg packetMsg)
        {
            // in order to get at the contents of the message, we need to create a ClientGCMsgProtobuf from the packet message we recieve
            // note here the difference between ClientGCMsgProtobuf and the ClientMsgProtobuf used when sending ClientGamesPlayed
            // this message is used for the GC, while the other is used for general steam messages
            var msg = new ClientGCMsgProtobuf<CMsgClientWelcome>(packetMsg);

            Console.WriteLine("GC is welcoming us. Version: {0}", msg.Body.version);

            // at this point, the GC is now ready to accept messages from us
            // so now we'll request the details of the match we're looking for

            CMsgPracticeLobbySetDetails details = new CMsgPracticeLobbySetDetails();
            details.game_name = "LOBBY";
            details.pass_key = "bscal";
            details.game_mode = (uint)DOTA_GameMode.DOTA_GAMEMODE_AP; // game mode
            details.server_region = (uint)ERegionCode.USEast;
            details.dota_tv_delay = LobbyDotaTVDelay.LobbyDotaTV_300;
            details.game_version = DOTAGameVersion.GAME_VERSION_CURRENT;
            details.visibility = DOTALobbyVisibility.DOTALobbyVisibility_Public;

            CreateLobby("bscal", details);
        }

        void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Console.WriteLine("Connected to Steam! Logging in '{0}'...", user);

            steamUser.LogOn(new SteamUser.LogOnDetails {
                Username = user,
                Password = pass,
            });

            pass = null;
        }

        void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Console.WriteLine("Disconnected from Steam");

            isRunning = false;
        }

        void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result != EResult.OK)
            {
                if (callback.Result == EResult.AccountLogonDenied)
                {
                    // if we recieve AccountLogonDenied or one of it's flavors (AccountLogonDeniedNoMailSent, etc)
                    // then the account we're logging into is SteamGuard protected
                    // see sample 5 for how SteamGuard can be handled

                    Console.WriteLine("Unable to logon to Steam: This account is SteamGuard protected.");

                    isRunning = false;
                    return;
                }

                Console.WriteLine("Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult);

                isRunning = false;
                return;
            }

            Console.WriteLine("Logged in! Launching DOTA...");

            // we've logged into the account
            // now we need to inform the steam server that we're playing dota (in order to receive GC messages)

            // steamkit doesn't expose the "play game" message through any handler, so we'll just send the message manually
            var playGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);

            playGame.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed {
                game_id = new GameID(APPID), // or game_id = APPID,
            });

            // send it off
            // notice here we're sending this message directly using the SteamClient
            steamClient.Send(playGame);

            // delay a little to give steam some time to establish a GC connection to us
            Thread.Sleep(5000);

            // inform the dota GC that we want a session
            var clientHello = new ClientGCMsgProtobuf<CMsgClientHello>((uint)EGCBaseClientMsg.k_EMsgGCClientHello);
            clientHello.Body.engine = ESourceEngine.k_ESE_Source2;
            gameCoordinator.Send(clientHello, APPID);

            // at this point, we'd be able to perform actions on Steam

            // for this sample we'll just log off
            // steamUser.LogOff();
        }

        void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Console.WriteLine("Logged off of Steam: {0}", callback.Result);
        }

        // this is a utility function to transform a uint emsg into a string that can be used to display the name
        string GetEMsgDisplayString(uint eMsg)
        {
            Type[] eMsgEnums =
            {
                typeof( EGCBaseClientMsg ),
                typeof( EDOTAGCMsg ),
                typeof( EGCBaseMsg ),
                typeof( EGCItemMsg ),
                typeof( ESOMsg ),
            };

            foreach (var enumType in eMsgEnums)
            {
                if (Enum.IsDefined(enumType, (int)eMsg))
                    return Enum.GetName(enumType, (int)eMsg);

            }

            return eMsg.ToString();
        }

        /// <summary>
        ///     Start the game
        /// </summary>
        public void LaunchLobby()
        {
            Send(new ClientGCMsgProtobuf<CMsgPracticeLobbyLaunch>((uint)EDOTAGCMsg.k_EMsgGCPracticeLobbyLaunch));
        }

        /// <summary>
        ///     Create a practice or tournament or custom lobby.
        /// </summary>
        /// <param name="passKey">Password for the lobby.</param>
        /// <param name="details">Lobby options.</param>
        public void CreateLobby(string passKey, CMsgPracticeLobbySetDetails details)
        {
            var create = new ClientGCMsgProtobuf<CMsgPracticeLobbyCreate>((uint)EDOTAGCMsg.k_EMsgGCPracticeLobbyCreate);
            create.Body.pass_key = passKey;
            create.Body.lobby_details = details;
            create.Body.lobby_details.pass_key = passKey;
            create.Body.lobby_details.visibility = DOTALobbyVisibility.DOTALobbyVisibility_Public;
            if (string.IsNullOrWhiteSpace(create.Body.search_key))
                create.Body.search_key = "";
            Send(create);
        }

        public void Send(IClientGCMsg msg)
        {
            var clientMsg = new ClientMsgProtobuf<CMsgGCClient>(EMsg.ClientToGC);

            clientMsg.Body.msgtype = MsgUtil.MakeGCMsg(msg.MsgType, msg.IsProto);
            clientMsg.Body.appid = (uint)APPID;

            clientMsg.Body.payload = msg.Serialize();

            steamClient.Send(clientMsg);
        }
    }
}
