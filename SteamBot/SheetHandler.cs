using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using SteamKit2;

namespace SteamBot
{
    class SheetHandler
    {

        /// <summary>
        /// New Method To get Worksheet
        /// </summary>
        /// <returns></returns>
        public  WorksheetEntry GetWorksheet(OAuth2Parameters parameters, string IntegrationName, string SpreadSheetURI, SpreadsheetsService service)
        {

            SpreadsheetQuery query = new SpreadsheetQuery(SpreadSheetURI);
            SpreadsheetFeed feed = service.Query(query);
            SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries[0];
            WorksheetFeed wsFeed = spreadsheet.Worksheets;
            WorksheetEntry worksheet = (WorksheetEntry)wsFeed.Entries[0];
            return worksheet;
        }


        //public void UploadSheet(bool Forcesync, Dictionary<string, Tuple<string, SteamID, string, bool>> Maplist, String IntegrationName, string CLIENT_ID,string CLIENT_SECRET, string REDIRECT_URI, string SCOPE, string GoogleAPI)
        public  void UploadSheet(bool Forcesync, Dictionary<string, Tuple<string, string, string, bool>> Maplist, OAuth2Parameters parameters, string IntegrationName, string SpreadSheetURI)
        {
            
            GOAuth2RequestFactory requestFactory = new GOAuth2RequestFactory(null, IntegrationName, parameters);
            SpreadsheetsService service = new SpreadsheetsService(IntegrationName);

            string accessToken = parameters.AccessToken;
            service.RequestFactory = requestFactory;

            WorksheetEntry worksheet = GetWorksheet(parameters, IntegrationName, SpreadSheetURI, service);

            worksheet.Rows = Convert.ToUInt32(Maplist.Count + 2);
            worksheet.Update();

            AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

            ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());

            ListFeed listFeed = service.Query(listQuery);

            int Increments = 1;

            if (listFeed.Entries.Count.ToString() == "1")
            {
                Increments = 0;
            }

            ListEntry NewRow = new ListEntry();

            foreach (var item in Maplist)
            {

                ListEntry row = (ListEntry)listFeed.Entries[Increments];

                row.Elements[1].Value = item.Key;
                row.Elements[2].Value = item.Value.Item1;
                row.Elements[3].Value = item.Value.Item2;
                row.Elements[4].Value = item.Value.Item3;
                row.Elements[5].Value = item.Value.Item4.ToString();

                row.Update();
                //service.Insert(listFeed, row);

                Increments = Increments + 1;
            }

            listFeed.Publish();

        }
        public  Dictionary<string, Tuple<string, string, string, bool>> SyncSheetDownload(string IntegrationName, OAuth2Parameters paramaters, string SpreadSheetURI)
        {
            
            GOAuth2RequestFactory requestFactory = new GOAuth2RequestFactory(null, IntegrationName, paramaters);
            SpreadsheetsService service = new SpreadsheetsService(IntegrationName);

            string accessToken = paramaters.AccessToken;
            service.RequestFactory = requestFactory;

            WorksheetEntry worksheet = GetWorksheet(paramaters, IntegrationName, SpreadSheetURI, service);

            Dictionary<string, Tuple<string, string, string, bool>> OnlineMapList = new Dictionary<string, Tuple<string, string, string, bool>>();

            string map = "MapNameError";
            string URL = "URL ERROR";
            string UserSteamID = "0";
            string Note = "No Notes";
            bool MapUploadStatus = false;

            AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

            ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
            ListFeed listFeed = service.Query(listQuery);

            foreach (ListEntry row in listFeed.Entries)
            {

                map = row.Elements[1].Value;
                URL = row.Elements[2].Value;
                UserSteamID = row.Elements[3].Value;
                Note = row.Elements[4].Value;

                if (row.Elements[5].Value == "TRUE")
                {
                    MapUploadStatus = true;
                }
                else
                {
                    MapUploadStatus = false;
                }
                if (!OnlineMapList.ContainsKey(map))
                {
                    OnlineMapList.Add(map, new Tuple<string, string, string, bool>(URL, UserSteamID, Note, MapUploadStatus));
                }

            }
            return OnlineMapList;

        }
    }
}
