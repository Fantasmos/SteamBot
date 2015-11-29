﻿
using SteamKit2;
using System.Collections.Generic;
using SteamTrade;
using SteamTrade.TradeWebAPI;
using System.Net;
using System.Xml;
//using System.ServiceModel;
//using System.ServiceModel.Web;
//using System.ServiceModel.Syndication;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Timers;
using SteamKit2.Internal;
using Newtonsoft.Json;
using Google.GData.Client;
using Google.GData.Spreadsheets;


namespace SteamBot
{
	
	public class GroupChatHandler : UserHandler
	{
		//JSON files store data, check if this works later. 
		double interval = 5000;
		//TODO ADD CHECK IF THE FILE EXISTS OR NOT
		public static string SettingsFile = "GroupChatHandler_Settings.json";
		public static Dictionary<string,string> groupchatsettings = JsonConvert.DeserializeObject<Dictionary<string,string>>(System.IO.File.ReadAllText(@SettingsFile));
		public string vdcCommand = groupchatsettings["vdcCommand"];
		public string tf2wCommand = groupchatsettings["tf2wCommand"];
		public string impCommand = groupchatsettings["impCommand"];
		public string mapListCommand = groupchatsettings["mapListCommand"];
		public string deletecommand = groupchatsettings["deletecommand"];
		public string clearcommand = groupchatsettings["clearcommand"];
		public string chatroomID = groupchatsettings["chatroomID"];
		public string tf2maps = groupchatsettings["tf2maps"];
		public string GoogleAPI = groupchatsettings["GoogleAPI"];
		public string CLIENT_ID = groupchatsettings["CLIENT_ID"];
		public string CLIENT_SECRET = groupchatsettings["CLIENT_SECRET"];
		public string SCOPE = groupchatsettings["SCOPE"];
		public string REDIRECT_URI = groupchatsettings["REDIRECT_URI"];
		public string OnlineSync = groupchatsettings["OnlineSync"];
		public string SpreadSheetURI =  groupchatsettings["SpreadSheetURI"]; //TODO CHANGE TO ID so you can just put in 1BGqQLnUFc2tO8NhALm7eLGlFRTSteMTGb5v4isVjK6o
		public string IntegrationName =  groupchatsettings["IntegrationName"];
		public string MapStoragePath =  groupchatsettings["MapStoragePath"];
		public string UserDatabase =  "users.json"; //Callback needs t be changed before making: groupchatsettings["UserDatabase"];
		public static string GroupchatID =  groupchatsettings["GroupchatID"];
		//public bool SpreadsheetSync = true;
		public static string PreviousMap1 = "";
		public static string PreviousMap2 = "";
		public static bool SpreadsheetSync = true;
		public static bool SyncRunning = false;
		public SteamID OtherSID { get; private set; } //check


		SteamID Groupchat = ulong.Parse (GroupchatID);

		private Timer Tick;


		//initialises the timer for the TickTasks() method to execute on
		public void InitTimer()
		{
			Tick = new Timer();
			Tick.Elapsed += new ElapsedEventHandler(TickTasks);
			Tick.Interval = interval; // in miliseconds
			Tick.Start();
		}
			
		//When the timer is 'ticked'
		//TODO make this a foreach method
		public void TickTasks(object sender, EventArgs e)
		{
			System.Net.IPAddress ipaddress1 = System.Net.IPAddress.Parse("70.42.74.31");  
			System.Net.IPAddress ipaddress2 = System.Net.IPAddress.Parse("91.121.155.109");

			Steam.Query.ServerInfoResult Map1 = ServerQuery (ipaddress1, 27015);
			Steam.Query.ServerInfoResult Map2 = ServerQuery (ipaddress2, 27015);

			Tuple<string,SteamID> Mapremoval = ImpRemove (Map1.Map, 0, true);
			Tuple<string,SteamID> Mapremoval2 = ImpRemove (Map2.Map, 0, true);

			if (Map1.Map != PreviousMap1 && Map1.Players > 2) {
				Bot.SteamFriends.SendChatMessage (Groupchat, EChatEntryType.ChatMsg, "Map changed to: " + Map1.Map.ToString() + " " + Map1.Players + "/" + Map1.MaxPlayers);
			}
			if (Map2.Map != PreviousMap2 && Map2.Players > 2) {
				Bot.SteamFriends.SendChatMessage (Groupchat, EChatEntryType.ChatMsg, "Map changed to: " + Map2.Map.ToString() + " " + Map2.Players + "/" + Map2.MaxPlayers);
			}
			PreviousMap1 = Map1.Map;
			PreviousMap2 = Map2.Map;

			if (Mapremoval.Item2 != 0) {
				Bot.SteamFriends.SendChatMessage (Mapremoval.Item2, EChatEntryType.ChatMsg, "Hi, your map: " + Mapremoval.Item1 + " is being imp'd!");
			}
			if (Mapremoval2.Item2 != 0){
				Bot.SteamFriends.SendChatMessage (Mapremoval2.Item2, EChatEntryType.ChatMsg, "Hi, your map: " + Mapremoval2.Item1 + " is being imp'd!");
			}

			if (SpreadsheetSync) 
			{
				SpreadsheetSync = false;
				SheetSync();
			}
				
		}
			
		//TODO add file decleration here for maps.json and stuff, so we can later implement per-chat file storage.
		//TODO add a lot more values, make it rely more on this settings file and make a template. This aint your bot forever

		/// <summary>
		/// Queries the server and returns the information
		/// </summary>
		/// <param name="ipadress">Ipadress that will be queried</param>
		/// <param name="port"> The port that will be used, typically 27015 </param> 
		Steam.Query.ServerInfoResult ServerQuery (System.Net.IPAddress ipaddress , Int32 port)
		{
			//System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse("70.42.74.31");  
			IPEndPoint ServerIP = new IPEndPoint( ipaddress , port);
			Steam.Query.Server Information = new Steam.Query.Server (ServerIP);
		    Steam.Query.ServerInfoResult ServerInformation = Information.GetServerInfo ().Result;
			return ServerInformation;
		}
		///<summary>Prints the Input to the interface</summary>
		public void LogRSS(string par1)
		{
			Log.Interface (par1);
		}

		public GroupChatHandler (Bot bot, SteamID sid) : base(bot, sid) {}

		public override void OnFriendRemove () {}

		public override void OnTradeAddItem (Schema.Item schemaItem, Inventory.Item inventoryItem) {}

		public override void OnTradeRemoveItem (Schema.Item schemaItem, Inventory.Item inventoryItem) {}

		public override void OnTradeMessage (string message) {}

		public override void OnTradeReady (bool ready) {}

		public override void OnTradeError (string error) {}

		public override void OnTradeTimeout () {}

		public override void OnTradeInit() {}

		public override bool OnTradeRequest() {return false;}

		public override void OnTradeSuccess(){}

		public override void OnTradeAwaitingEmailConfirmation(long tradeOfferID){}

		public override void OnTradeAccept(){}

		public override bool OnGroupAdd()
		{
			return false;
		}

		public override bool OnFriendAdd () 
		{
			return false;
		}

		public SteamKit2.SteamFriends SteamFriends;
		public void OnChatEnter(SteamKit2.SteamFriends.ChatEnterCallback callback){
			Log.Interface ("Entered Chat");
		}
			
		public override void OnLoginCompleted()
		{
			Bot.SteamFriends.JoinChat (new SteamID (Groupchat));
			InitTimer ();
		}
		public override void OnBotCommand(string command)
		{
			if (command.StartsWith ("Server_Q" , StringComparison.OrdinalIgnoreCase)) 
			{
				//ServerQuery ();
			}
			if (command.StartsWith ("Say" , StringComparison.OrdinalIgnoreCase)) 
			{
				string send = command.Remove (0, 3);
				Bot.SteamFriends.SendChatRoomMessage (Groupchat, EChatEntryType.ChatMsg, send); //Posts to the chat the entry put in by the bot
			}
			if (command.StartsWith ("Google" , StringComparison.OrdinalIgnoreCase)) 
			{
				SheetSync();
			}
			if (command.StartsWith ("ID" , StringComparison.OrdinalIgnoreCase)) 
			{
				Log.Interface (OtherSID.ToString ());
			}
			if (command.StartsWith("!REAUTH" , StringComparison.OrdinalIgnoreCase)) 
				{
				OAuth2Parameters parameters = new OAuth2Parameters ();

				parameters.ClientId = CLIENT_ID;

				parameters.ClientSecret = CLIENT_SECRET;

				parameters.RedirectUri = REDIRECT_URI;

				parameters.Scope = SCOPE;
					//AUTHENTICATE
					string authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl (parameters);
					Log.Interface (authorizationUrl);
				    Log.Info (authorizationUrl);
				    Log.Debug (authorizationUrl);
				   Log.Interface ("Please visit the URL above to authorize your OAuth "
						+ "request token.  Once that is complete, type in your access code to "
						+ "continue...");
					parameters.AccessCode = Console.ReadLine ();
				Dictionary<string,string> entrydata = JsonConvert.DeserializeObject<Dictionary<string,string>>(System.IO.File.ReadAllText(@SettingsFile));
				OAuthUtil.GetAccessToken (parameters);
				string refreshToken = parameters.RefreshToken;
				entrydata.Remove ("GoogleAPI");
				entrydata.Add ("GoogleAPI", refreshToken);
				System.IO.File.WriteAllText (@SettingsFile, JsonConvert.SerializeObject (entrydata));
				GoogleAPI = refreshToken;
				Log.Interface ("SYNC COMMANDS WILL NOT WORK UNTIL BOT RESTART, PLEASE RESTART");
				} 
		}

		//TODO ALLOW USERS TO SEND ADMIN COMMANDS VIA MSG
		public override void OnMessage (string message, EChatEntryType type) {
			SteamID ChatMsg = 0;
			string adminresponse = null;
			string response = Chatcommands (ChatMsg, ChatMsg, message);

		    if (response != null) 
			{
				SendChatMessage (response);
			}

			//Log.Interface(OtherSID.ToString() + ": " + message ); //TODO EXTEND THIS FUCNTIONALITY

			//if (admincheck(OtherSID)){
			//	adminresponse = admincommands (OtherSID, message);
			//}
			//if (adminresponse != null) {
			//	SendChatMessage (adminresponse);
			//}
		}
		public override void OnChatRoomMessage(SteamID chatID, SteamID sender, string message)
		{
			MapVolume ();
			string adminresponse = null;
			if (admincheck(sender)){
				adminresponse = admincommands (sender, message);
			}
			string response = Chatcommands(chatID, sender, message);
			if (response != null) {
				Bot.SteamFriends.SendChatRoomMessage (Groupchat, EChatEntryType.ChatMsg, response);
			}
			if (adminresponse != null) {
				Bot.SteamFriends.SendChatRoomMessage (Groupchat, EChatEntryType.ChatMsg, adminresponse);
			}
		}

		/// <summary>
		/// The commands that users can use by msg'ing the system. Returns a string with the appropriate responses
		/// </summary>
		/// <param name="chatID">ChatID of the chatroom</param>
		/// <param name="sender">STEAMID of the sender</param>
		/// <param name="message">The message sent</param>
		public string admincommands (SteamID sender, string message)
		{
			
			if (message.StartsWith (clearcommand, StringComparison.OrdinalIgnoreCase)) 
			{
				string path = @MapStoragePath;
				File.Delete(path);
				File.WriteAllText (path, "{}");
				//SheetSync ();
				SpreadsheetSync = true;
				return "Wiped all Maps";
			}
			if (message.StartsWith ("!EnableSync", StringComparison.OrdinalIgnoreCase)) {
				Dictionary<string,string> entrydata = JsonConvert.DeserializeObject<Dictionary<string,string>> (System.IO.File.ReadAllText (@SettingsFile));
				//if it already exists, it deletes it so it can update the data
				entrydata.Remove ("OnlineSync");
				entrydata.Add ("OnlineSync", "true");
				System.IO.File.WriteAllText (@SettingsFile, JsonConvert.SerializeObject (entrydata));
				OnlineSync = "true";
				return "Enabled Sync";
			}
			if (message.StartsWith ("!DisableSync", StringComparison.OrdinalIgnoreCase)) {
				Dictionary<string,string> entrydata = JsonConvert.DeserializeObject<Dictionary<string,string>> (System.IO.File.ReadAllText (@SettingsFile));
				//if it already exists, it deletes it so it can update the data
				entrydata.Remove ("OnlineSync");
				entrydata.Add ("OnlineSync", "false");
				System.IO.File.WriteAllText (@SettingsFile, JsonConvert.SerializeObject (entrydata));
				OnlineSync = "false";
				return "Disabled Sync";
			}

			return null;
		}
		/// <summary>
		/// The commands that users can use by msg'ing the system. Returns a string with the appropriate responses
		/// </summary>
		/// <param name="chatID">ChatID of the chatroom</param>
		/// <param name="sender">STEAMID of the sender</param>
		/// <param name="message">The message sent</param>
		public string Chatcommands (SteamID chatID, SteamID sender, string message)
		{
			base.OnChatRoomMessage (chatID, sender, message);
			bool rank = admincheck(sender);
			Log.Interface(Bot.SteamFriends.GetFriendPersonaName(sender) + ":" + "(" + rank + ")" + " " + message); 
			Log.Info (Bot.SteamFriends.GetFriendPersonaName (sender) + ": " + message);
			//Retrieves the database of users, checks if they're an admin, and enables admin only commands between brackets
			if (rank) { 
			}

			if (message.StartsWith (vdcCommand , StringComparison.OrdinalIgnoreCase)) 
			{
				string par1 = message.Remove (0, 5);
				return GoogleSearch(par1, "https://developer.valvesoftware.com/", chatID);
			}
			if (message.StartsWith ("!Sync", StringComparison.OrdinalIgnoreCase)) 
			{
				SheetSync ();
				return null;
			}

			if (message.StartsWith (tf2maps , StringComparison.OrdinalIgnoreCase)) 
			{
				string par1 = message.Remove (0, 4);
				return GoogleSearch(par1, "http://tf2maps.net/", chatID);
			}

			if (message.StartsWith ("You have a leak" , StringComparison.OrdinalIgnoreCase)) 
			{
				string output = "https://developer.valvesoftware.com/wiki/leak";
				return output;
				//Bot.SteamFriends.SendChatRoomMessage (103582791429594873, EChatEntryType.ChatMsg, output);
			}
			if (message.StartsWith (tf2wCommand , StringComparison.OrdinalIgnoreCase)) 
			{
				string par1 = message.Remove (0, 5);
				return GoogleSearch(par1, "https://wiki.teamfortress.com/", chatID);
			}
			if (message.StartsWith ("ghost?" , StringComparison.OrdinalIgnoreCase)) 
			{
				return "no";
			//	Bot.SteamFriends.SendChatRoomMessage (chatID, EChatEntryType.ChatMsg, "no");
			}
			if (message.StartsWith ("!help" , StringComparison.OrdinalIgnoreCase)) 
			{
				return "http://tf2maps.net/threads/we-now-have-a-steam-chat-bot.26274/";
			}

			if (message.StartsWith ("!RSS" , StringComparison.OrdinalIgnoreCase)) 
			{
				//string url = "http://steamcommunity.com/groups/TF2Mappers/rss/";
				//XmlReader reader = XmlReader.Create(url);
				//SyndicationFeed feed = SyndicationFeed.Load(reader);
				//string file = reader.ToString();
				//reader.Close();
				//var latest = feed.Items.OrderByDescending(x=>x.PublishDate).FirstOrDefault().Title.Text;
				//Log.Interface (latest.ToString());
				//SendChatMessage(latest.ToString());
				//Bot.SteamFriends.SendChatRoomMessage (chatID, EChatEntryType.ChatMsg, latest.ToString());
			}
			if (message.StartsWith(deletecommand ,  StringComparison.OrdinalIgnoreCase))
			{
				string map = message.Remove (0, 5);
				Tuple<string,SteamID> removed = ImpRemove (map, sender, false);
				//SheetSync ();
				return "Removed map: " + removed.Item1;
				//Bot.SteamFriends.SendChatRoomMessage (chatID, EChatEntryType.ChatMsg, removed);
			}	
		
			if (message.StartsWith (impCommand , StringComparison.OrdinalIgnoreCase)) 
			{
				string[] words = message.Split(' ');
				//Log.Interface (words.LongLength.ToString () + " " + words.Length.ToString());
				if (words.Length < 3)
				{
					return "Missing commands! Please use the format: !add Mapname DownloadUrl";
				} 
				else 
				{
					string[] notes = message.Split (new string[] {words [2]}, StringSplitOptions.None);


					if (notes.Length < 2) {
						notes [1] = "No notes";
					}
					string status = ImpEntry (words [1], words [2], notes [1], sender);
					SpreadsheetSync = true;
					//SheetSync();
					return status;
				}
			}
			if (message.StartsWith ("!Sheet", StringComparison.OrdinalIgnoreCase)) {
				return "https://goo.gl/Q5bQxg";
			}
			if (message.StartsWith (mapListCommand , StringComparison.OrdinalIgnoreCase)) 
			{
				Dictionary<string,Tuple<string,SteamID> > entrydata = JsonConvert.DeserializeObject<Dictionary<string,Tuple<string,SteamID>>>(System.IO.File.ReadAllText(@MapStoragePath));
				string Maplisting = "";
				string DownloadListing = "";
				foreach (var item in entrydata) 
				{
					Maplisting = Maplisting + item.Key + " , ";
					DownloadListing = DownloadListing + item.Value.Item1 + " , ";
				}
				Bot.SteamFriends.SendChatMessage (sender, EChatEntryType.ChatMsg, DownloadListing);
				return Maplisting ;
				//Bot.SteamFriends.SendChatMessage (sender, EChatEntryType.ChatMsg, DownloadListing);
				//Bot.SteamFriends.SendChatRoomMessage (chatID, EChatEntryType.ChatMsg, Maplisting);
			}
			return null;
		}
		/// <summary>
		/// Adds a map to the database
		/// </summary>
		public string ImpEntry(string map , string downloadurl , string notes, SteamID sender)
		{
			//Deserialises the current map list
			string response = "Failed to add the map to the list";
			Dictionary<string,Tuple<string,SteamID,string> > entrydata = new Dictionary<string,Tuple<string,SteamID,string>>();
			if (JsonConvert.DeserializeObject<Dictionary<string,Tuple<string,SteamID,string>>> (System.IO.File.ReadAllText (@MapStoragePath)) != null) 
			{
				entrydata = JsonConvert.DeserializeObject<Dictionary<string,Tuple<string,SteamID,string>>> (System.IO.File.ReadAllText (@MapStoragePath));
			} 

			if (entrydata.ContainsKey (map)) { //if it already exists, it deletes it so it can update the data
				Log.Interface("duplicate");
				response = "Error, the entry already exists! Please remove the existing entry";
			} else {
				//Adds the entry
				entrydata.Add (map, new Tuple<string,SteamID,string> (downloadurl, sender, notes));
				//Saves the data
				System.IO.File.WriteAllText (@MapStoragePath, JsonConvert.SerializeObject (entrydata));
				response = "Added " + map; 
			}
			SpreadsheetSync = true;
			return response;
		}
		/// <summary>
		/// Removes specified map from the database.
		/// Checks if the user is an admin or the setter
		/// </summary>
		public Tuple<string,SteamID> ImpRemove (string map , SteamID sender , bool ServerRemove)
		{
			Dictionary<string,Tuple<string,SteamID,string> > Maplist = JsonConvert.DeserializeObject<Dictionary<string,Tuple<string,SteamID,string>>> (System.IO.File.ReadAllText (@MapStoragePath));
			Dictionary<string,Tuple<string,SteamID,string> > NewMaplist = new Dictionary<string, Tuple<string, SteamID,string>>();
			string removed = "The map was not found or you do not have sufficient privileges"; 
			SteamID userremoved = 0;
			foreach (var item in Maplist) {
				//TODO DEBUG
				if (item.Key == map && (admincheck (sender) || sender == item.Value.Item2 || ServerRemove )) {
					removed = map;
					userremoved = item.Value.Item2;
					SpreadsheetSync = true;
				} else {
					NewMaplist.Add (item.Key, item.Value);
				}
			}
			System.IO.File.WriteAllText(@MapStoragePath, JsonConvert.SerializeObject(NewMaplist));
			Tuple<string,SteamID> RemoveInformation = new Tuple<string,SteamID> (removed, userremoved); 
			return RemoveInformation ;
			{
			}
		}
		///<summary> Checks if the given STEAMID is an admin in the database</summary>
		public bool admincheck(SteamID sender)
		{
			//string filedata = System.IO.File.ReadAllText(@"users.json");
			Dictionary<string,EClanPermission> Dictionary = JsonConvert.DeserializeObject<Dictionary<string,EClanPermission>>(System.IO.File.ReadAllText(@UserDatabase));
			if (Dictionary.ContainsKey (sender.ToString ())) { //If the STEAMID is in the dictionary
				string Key = sender.ToString (); 
				EClanPermission UserPermissions = Dictionary [Key]; //It will get the permissions value
				if((UserPermissions & EClanPermission.OwnerOfficerModerator) > 0) //Checks if it has sufficient privilages
				{
					return true; //If there's sufficient privilages, it'll return true
				}
			}
			return false; //If there is no entry in the database, or there aren't sufficient privalages, it'll return false
		}

		/// <summary>
		/// Makes a google search, and restricts results to only a single URL.
		/// Returns the URL of the first result
		/// </summary>
		// TODO Clean up the code so it properly uses the google API and is a return function
		public string GoogleSearch(string searchquerey , string url, SteamID chatID) {
			WebClient client = new WebClient ();
			string search = "http://www.google.com.au/search?q=" + searchquerey + "+site:" + url;
			string httpdata = client.DownloadString (search);
			string[] suffix = httpdata.Split (new string[] { "<h3 class=\"r\"><a href=\"/url?q=" + url }, System.StringSplitOptions.None);
			Log.Info (httpdata);
			if (suffix.LongLength > 1) {
				string suffix_string = suffix [1];
				string[] suffix_split = suffix_string.Split (new string[] { "&" }, System.StringSplitOptions.None);
				string page = suffix_split [0];
				string output = url + page;
				Log.Info ("requested:" + output);
				return output;
			} 
			else 
			{
				return "Invalid Search";
			}
		}
			
		//TODO Phase this system out slowly and replace with the existing
		public static void maps(string map)
		{
			string path = @"logs\maps.log";

			// This text is added only once to the file.
			if (!File.Exists (path)) {
				// Create a file to write to.
				string createText = map;
				File.WriteAllText (path, createText + " , ");
			} 
			else 
			{
				// This text is always added, making the file longer over time
				// if it is not deleted.
				string appendText = map + " , ";
				File.AppendAllText (path, appendText);

				// Open the file to read from.
				string readText = File.ReadAllText (path);
				Console.WriteLine (readText);
			}
		}
		/// <summary>
		/// Updates the online spreadsheet according the maps file
		/// </summary>
		/// 
		//TODO Clean portions that need cleaning
		public void SheetSync ()
		{
			Dictionary<string,Tuple<string,SteamID, string> > entrydata = JsonConvert.DeserializeObject<Dictionary<string,Tuple<string,SteamID,string>>>(System.IO.File.ReadAllText(@MapStoragePath));
			Bot.SteamFriends.SetPersonaName ("[" + entrydata.Count.ToString() + "] " + Bot.DisplayName);
			Log.Interface ("Initialising sync");
			if (OnlineSync.StartsWith ("true", StringComparison.OrdinalIgnoreCase) && !SyncRunning) {
				SyncRunning = true;
				Log.Interface ("BEGINNING ONLINE SYNC");
			
				OAuth2Parameters parameters = new OAuth2Parameters ();
				parameters.ClientId = CLIENT_ID;
				parameters.ClientSecret = CLIENT_SECRET;
				parameters.RedirectUri = REDIRECT_URI;
				parameters.Scope = SCOPE;
				//parameters.AccessToken = GoogleAPI;
				parameters.AccessType = "offline";
				parameters.RefreshToken = GoogleAPI;
				OAuthUtil.RefreshAccessToken (parameters);
				string accessToken = parameters.AccessToken;

				GOAuth2RequestFactory requestFactory = new GOAuth2RequestFactory (null, IntegrationName, parameters);
				SpreadsheetsService service = new SpreadsheetsService (IntegrationName);
				service.RequestFactory = requestFactory;

				//TODO Put this in the settings file, and have his also be the result from !sheet
				SpreadsheetQuery query = new SpreadsheetQuery (SpreadSheetURI);

				SpreadsheetFeed feed = service.Query (query);
				SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries [0];
				WorksheetFeed wsFeed = spreadsheet.Worksheets;
				WorksheetEntry worksheet = (WorksheetEntry)wsFeed.Entries [0];
	
				//Sets the size of the dictionary to the size of the JSON file, and updates
				worksheet.Cols = 4;

				if (entrydata.Count + 2 > worksheet.RowCount.IntegerValue) {
					worksheet.Rows = Convert.ToUInt32 (entrydata.Count + 2);
				}

				worksheet.Update ();

				CellQuery cellQuery = new CellQuery (worksheet.CellFeedLink);
				cellQuery.ReturnEmpty = ReturnEmptyCells.yes;
				CellFeed cellFeed = service.Query (cellQuery);

				//TODO Updates the Bot Prefix to the map list
				//SteamFriends.SetPersonaName ("[" + entrydata.Count.ToString() + "] " + Bot.DisplayName);


				//Makes the current Cells empty, to be replaced by the existing Maps list
				foreach (CellEntry cell in cellFeed.Entries) {
					cell.InputValue = " ";
					cell.Update ();
				}
				// Iterate through each cell, updating its value if necessary. TODO Optimise this function to use batch upload, or just generally faster.

				int Entries = 1;

				foreach (var item in entrydata) {
					Entries = Entries + 1; 
					foreach (CellEntry cell in cellFeed.Entries) {
						if (cell.Title.Text == "A" + Entries.ToString ()) {
							cell.InputValue = item.Key;
							cell.Update ();
						}
						if (cell.Title.Text == "B" + Entries.ToString ()) {
							cell.InputValue = item.Value.Item1;
							cell.Update ();
						}
						if (cell.Title.Text == "C" + Entries.ToString ()) {
							cell.InputValue = item.Value.Item2.ToString ();
							cell.Update ();
						}
						if (cell.Title.Text == "D" + Entries.ToString ()) {
							cell.InputValue = item.Value.Item3.ToString ();
							cell.Update ();
						}
					
					
					}
				}
				worksheet.Rows = Convert.ToUInt32 (entrydata.Count + 2);
				worksheet.Update ();
				SyncRunning = false;
			} else if (OnlineSync.StartsWith ("true", StringComparison.OrdinalIgnoreCase)){
				SpreadsheetSync = true;
			}
		}

		public string MapVolume ()
		{
			Dictionary<string,Tuple<string,SteamID,string> > entrydata = JsonConvert.DeserializeObject<Dictionary<string,Tuple<string,SteamID,string>>>(System.IO.File.ReadAllText(@MapStoragePath));
			return entrydata.Count.ToString ();
		}
	
		//TODO update all code below to no longer be obsolete



		//The list of feeds that will be updated, where each will be saved, and if they will be posted to chat, obsolete
		public void rssfeedupdates()
		{
			rssloop("https://www.reddit.com/r/AskReddit/new/.rss" , "Debug.log" , false);
			rssloop("http://steamcommunity.com/groups/TF2Mappers/rss/" , "GroupEvents.log" , true);
			rssloop("http://steamcommunity.com/groups/tf2maps_twitter/rss/" , "twitterbot.log" , true);
			rssloop("http://www.teamfortress.com/rss.xml", "tf2site.log", true);
		}

		//Gets the RSS feed, then saves it
		public void rssloop (string feed , string filename , Boolean sendtochat)
		{
			getrssfirstentry (feed);
			string latest = getrssfirstentry(feed);
			RSSWrite (@"logs\", filename, latest, sendtochat);
		}

		//Gets the feed sent, and returns the first entry
		string getrssfirstentry(string feedtoread)
		{
			//	string url = feedtoread ;
			//	XmlReader reader = XmlReader.Create(url);
			//	SyndicationFeed feed = SyndicationFeed.Load(reader);
			//	string file = reader.ToString();
			// Closing this, will it change anything? reader.Close();
			//	var latest = feed.Items.FirstOrDefault().Title.Text;
			return "THIS HAS BEEN FROZEN DUE TO AN ERROR";
		}

		//Will compare the LOG file with the RSS feed's first entry.
		public void RSSWrite (string path , string filename , string content, Boolean SendtoChat)
		{
			string filepath = path + filename;
			if (!File.Exists (filepath)) //Checks if the RSS feed is stored already, if not it writes it
			{
				File.WriteAllText (filepath, content);
			}
			else
			{
				string readText = File.ReadAllText (filepath);
				if (content != readText)
				{ //Checks if the existing entry is the same or different
					File.WriteAllText (filepath, content);
					LogRSS ("New Entry");
					LogRSS (content);
					if (SendtoChat == true)
					{
						Bot.SteamFriends.SendChatRoomMessage (Groupchat, EChatEntryType.ChatMsg, content); //Posts to the chat the new entry if it's set to
					}
				}
				else
				{
					LogRSS ("No new entry"); //Logs that there is no new entry
				}
			}
		}
	}
}

