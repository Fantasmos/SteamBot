using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Google.GData.Client;
using Google.GData.Spreadsheets;

using System.Collections.Generic;

namespace Steambot.tests
{
    [TestClass]
    public class UnitTest1
    {
        /* [TestMethod]
        public void UploadDownloadTests()
        {
            OAuthUtil.RefreshAccessToken(SteamBot.GroupChatHandler.OauthParameters);
            //Arrange
            //Initialise a dictionary to upload, then upload it. It will later be used to compare the downloaded sheet alongside
            Dictionary<string, Tuple<string, string, string, bool>> ExpectedOutput = new Dictionary<string, Tuple<string, string, string, bool>>();
            Tuple<string, string, string, bool> TestTuple = Tuple.Create<string, string, string, bool>("data3", "data5", "data2", true);
            ExpectedOutput.Add("MapName2", TestTuple);

            //Act
            //Upload The sheet. Then extract the data from the server into a new sheet
            new SteamBot.SheetService().UploadSheet(true, ExpectedOutput, SteamBot.GroupChatHandler.OauthParameters, "MySpreadsheetIntegration-v1", "https://spreadsheets.google.com/feeds/spreadsheets/private/full/1BGqQLnUFc2tO8NhALm7eLGlFRTSteMTGb5v4isVjK6o");

            Dictionary<string, Tuple<string, string, string, bool>> ActualOutput = new SteamBot.SheetService().SheetDownload("MySpreadsheetIntegration-v1", SteamBot.GroupChatHandler.OauthParameters, "https://spreadsheets.google.com/feeds/spreadsheets/private/full/1BGqQLnUFc2tO8NhALm7eLGlFRTSteMTGb5v4isVjK6o");

            foreach (KeyValuePair<string, Tuple<string, string, string, bool>> ActualOutputEntry in ActualOutput)
            {
                Console.WriteLine(ActualOutputEntry.Key);
                Console.WriteLine(ActualOutputEntry.Value);
            }

            //assert
            //Iterate though each value in the Original Dictionary, and ensure that they are contained in the new sheet. Then Verify their values are equal 
            foreach (KeyValuePair<string, Tuple<string, string, string, bool>> ExpectedOutputEntry in ExpectedOutput)
            {
                Console.WriteLine("Expected Output Key: " + ExpectedOutputEntry.Key);

                Console.WriteLine("Values in Expected Output: " + ExpectedOutput[ExpectedOutputEntry.Key]);
                Console.WriteLine("If Crash exists here, then the key is not shared in both dictionaries");
                Console.WriteLine("Values in actual Output: " + ActualOutput[ExpectedOutputEntry.Key]);


                if (((ActualOutput.ContainsKey(ExpectedOutputEntry.Key))))
                {

                    Console.WriteLine("Key Does exist");
                    Console.WriteLine(ActualOutput[ExpectedOutputEntry.Key]);
                    Console.WriteLine(ExpectedOutput[ExpectedOutputEntry.Key]);
                    if (((ActualOutput.ContainsKey(ExpectedOutputEntry.Key)) & (ActualOutput[ExpectedOutputEntry.Key].Equals(ExpectedOutput[ExpectedOutputEntry.Key]))) == false)
                    {
                        Assert.Fail();
                    }

                }
                else
                {
                    Assert.Fail();
                }
            }




        }

        //Sheet Synchronisation Unit Tests
        [TestMethod]
        public void SyncrhonisationTest()
        {
            //Arrange
            Dictionary<string, Tuple<string, string, string, bool>> MapList1 = new Dictionary<string, Tuple<string, string, string, bool>>();
            Tuple<string, string, string, bool> TestTuple1 = Tuple.Create<string, string, string, bool>("data11", "data12", "data13", true);
            MapList1.Add("MapName1", TestTuple1);

            Dictionary<string, Tuple<string, string, string, bool>> MapList2 = new Dictionary<string, Tuple<string, string, string, bool>>();
            Tuple<string, string, string, bool> TestTuple2 = Tuple.Create<string, string, string, bool>("data21", "data22", "data23", true);
            MapList2.Add("MapName2", TestTuple2);



            Dictionary<string, Tuple<string, string, string, bool>> ExpectedOutput = new Dictionary<string, Tuple<string, string, string, bool>>();
            ExpectedOutput.Add("MapName1", TestTuple1);
            ExpectedOutput.Add("MapName2", TestTuple2);



            //Act
            
            Dictionary<string, Tuple<string, string, string, bool>> ActualOutput = new SteamBot.SheetService().SyncrhoniseDictionaries(null, MapList1, MapList2);

            //Assert

            //Iterates through each value within the expectedOutput, comparing it to the actual output and failing on any differences
            foreach (KeyValuePair<string, Tuple<string, string, string, bool>> ExpectedOutputEntry in ExpectedOutput)
            {
                Console.WriteLine("Expected Output Key: " + ExpectedOutputEntry.Key);

                Console.WriteLine("Values in Expected Output: " + ExpectedOutput[ExpectedOutputEntry.Key]);
                Console.WriteLine("If Crash exists here, then the key is not shared in both dictionaries");
                Console.WriteLine("Values in actual Output: " + ActualOutput[ExpectedOutputEntry.Key]);


                if (((ActualOutput.ContainsKey(ExpectedOutputEntry.Key)) & (ActualOutput[ExpectedOutputEntry.Key].Equals(ExpectedOutput[ExpectedOutputEntry.Key]))) == false)
                {
                    Assert.Fail();
                }
            }

            //Test 2
            Console.WriteLine("Test 2, to Ensure that the map to remove, is removed");
            //Assert
            MapList2.Add("MapName3", TestTuple2);
            string MapToRemove = "MapName3";

            ActualOutput = new SteamBot.SheetService().SyncrhoniseDictionaries(MapToRemove, MapList1, MapList2);


            //Ensures that the map to remove, is removed
            foreach (KeyValuePair<string, Tuple<string, string, string, bool>> Entry in ActualOutput)
            {
                Console.WriteLine("Values in actual Output: " + Entry.Key);
                if (Entry.Key == MapToRemove)
                {
                    Assert.Fail();
                }
            }
            //Test 3 Ensures removal of duplicates
            Console.WriteLine();
            Console.WriteLine("Test 3: Ensure the removal of duplicates");



            MapList2.Add("MapName1", TestTuple1);
            
            ActualOutput = new SteamBot.SheetService().SyncrhoniseDictionaries(null, MapList1, MapList2);

            foreach (KeyValuePair<string, Tuple<string, string, string, bool>> ExpectedOutputEntry in ExpectedOutput)
            {
                Console.WriteLine("Expected Output Key: " + ExpectedOutputEntry.Key);

                Console.WriteLine("Values in Expected Output: " + ExpectedOutput[ExpectedOutputEntry.Key]);
                Console.WriteLine("If Crash exists here, then the key is not shared in both dictionaries");
                Console.WriteLine("Values in actual Output: " + ActualOutput[ExpectedOutputEntry.Key]);


                if (((ActualOutput.ContainsKey(ExpectedOutputEntry.Key)) & (ActualOutput[ExpectedOutputEntry.Key].Equals(ExpectedOutput[ExpectedOutputEntry.Key]))) == false)
                {
                    Assert.Fail();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Print all from new list:");

            foreach (KeyValuePair<string, Tuple<string, string, string, bool>> ActualOutputEntry in ActualOutput)
            {
                Console.WriteLine(ActualOutputEntry.Key);
            }

            //Test 4 Ensures all issues are not apparent by combining all of them at once 
            Console.WriteLine();
            Console.WriteLine("Ensures that the sheets are as expected, duplicates are removed, and the map to remove is removed");

            ActualOutput = new SteamBot.SheetService().SyncrhoniseDictionaries(MapToRemove, MapList1, MapList2);

            foreach (KeyValuePair<string, Tuple<string, string, string, bool>> ExpectedOutputEntry in ExpectedOutput)
            {
                Console.WriteLine("Expected Output Key: " + ExpectedOutputEntry.Key);

                Console.WriteLine("Values in Expected Output: " + ExpectedOutput[ExpectedOutputEntry.Key]);
                Console.WriteLine("If Crash exists here, then the key is not shared in both dictionaries");
                Console.WriteLine("Values in actual Output: " + ActualOutput[ExpectedOutputEntry.Key]);


                if (((ActualOutput.ContainsKey(ExpectedOutputEntry.Key)) & (ActualOutput[ExpectedOutputEntry.Key].Equals(ExpectedOutput[ExpectedOutputEntry.Key]))) == false)
                {
                    Assert.Fail();
                }
            }

            //Check if Values to delete are deleted
            foreach (KeyValuePair<string, Tuple<string, string, string, bool>> Entry in ActualOutput)
            {
                Console.WriteLine("Checking if" + MapToRemove + " = " + Entry.Key);
                if (Entry.Key == MapToRemove)
                {
                    Assert.Fail();
                }
            }
        }
    */}
}
