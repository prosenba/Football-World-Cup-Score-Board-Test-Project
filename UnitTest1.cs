using Football_World_Cup_Score_Board_Test.BLL;
using Microsoft.Extensions.Logging;
using Moq;
using Football_World_Cup_Score_Board_Test.BLL;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Football_World_Cup_Score_Board_Test.Controllers;
using Football_World_Cup_Score_Board_Test.Models;
using Microsoft.AspNetCore.Mvc;

namespace Football_World_Cup_Score_Board_Test_Project
{
    //The NuGet package you need for Moq in MSTest is "Moq.MSTest."
    //You can install it using the NuGet Package Manager in Visual Studio by searching for "Moq.MSTest"
    [TestClass]
    public class UnitTest1
    {
        //Test the ParseTeamStringListTests
        [TestClass]
        public class ParseTeamStringListTests
        {
            [TestMethod]
            public void ParseStringList_ShouldReturnCorrectTeamObjects()
            {
                // Arrange
                var logger = new Logger<ParseTeamStringList>(new LoggerFactory());
                var input = new List<string> { "Brazil vs Argentina 3:2", "Germany vs Spain 1:0" };
                var parseTeamStringList = new ParseTeamStringList(logger);
                // Act
                var result = parseTeamStringList.ParseStringList(input);
                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("Brazil", result[0].homeTeamName);
                Assert.AreEqual("Argentina", result[0].awayTeamName);
                Assert.AreEqual(3, result[0].homeTeamScore);
                Assert.AreEqual(2, result[0].awayTeamScore);
            }
        }



        [TestClass]
        public class ImportTeamDataTests
        {
            private ImportTeamData _importTeamData;

            [TestInitialize]
            public void TestInitialize()
            {
                // create a mock ILogger
                var mockLogger = new Mock<ILogger<ImportTeamData>>();
                _importTeamData = new ImportTeamData(mockLogger.Object);
            }

            [TestMethod]
            public void TestGetTeamDataList()
            {
                var teamDataList = _importTeamData.getTeamDataList();
                Assert.IsNotNull(teamDataList);
                Assert.AreEqual(5, teamDataList.Count);
                Assert.AreEqual("a.Mexico - Canada: 0 - 5", teamDataList[0]);
                Assert.AreEqual("b.Spain - Brazil: 10 – 2", teamDataList[1]);
                // ... add more assertions for other elements in the list
            }
        }


        // This uses xUnit, you will need to install the xUnit.net NuGet package.
        //This can be done by opening the Package Manager Console in Visual Studio and
        //running the command Install-Package xunit extensibility execution.

        public class SortTeamsListTest
        {
            [Fact]
            public void ReOrder_ShouldSortTeamsListByScoreAndDate()
            {
                // Arrange
                var teamsList = new List<Teams>()
            {
                new Teams { homeTeamScore = 2, awayTeamScore = 1, dateAdded = new DateTime(2022,6,1).ToString() },
                new Teams { homeTeamScore = 5, awayTeamScore = 0, dateAdded = new DateTime(2022,6,2).ToString() },
                new Teams { homeTeamScore = 1, awayTeamScore = 3, dateAdded = new DateTime(2022,6,3).ToString() },
                new Teams { homeTeamScore = 6, awayTeamScore = 6, dateAdded = new DateTime(2022,6,4).ToString() },
                new Teams { homeTeamScore = 2, awayTeamScore = 2, dateAdded = new DateTime(2022,6,5).ToString() },
            };

                var expectedTeamsList = new List<Teams>()
            {
                new Teams { homeTeamScore = 6, awayTeamScore = 6, dateAdded = new DateTime(2022,6,4).ToString() },
                new Teams { homeTeamScore = 2, awayTeamScore = 1, dateAdded = new DateTime(2022,6,1).ToString() },
                new Teams { homeTeamScore = 1, awayTeamScore = 3, dateAdded = new DateTime(2022,6,3).ToString() },
                new Teams { homeTeamScore = 2, awayTeamScore = 2, dateAdded = new DateTime(2022,6,5).ToString() },
                new Teams { homeTeamScore = 5, awayTeamScore = 0, dateAdded = new DateTime(2022,6,2).ToString() },
            };

                var mockLogger = new Mock<ILogger<SortTeamsList>>();

                var sortTeamsList = new SortTeamsList(mockLogger.Object);

                // Act checks if list are the same afte sort
                var result = sortTeamsList.ReOrder(teamsList);

                // Assert
                Assert.AreEqual(expectedTeamsList, result);
                mockLogger.Verify(logger => logger.LogInformation("ReOrder Medthod Called"), Times.Once);
                mockLogger.Verify(logger => logger.LogTrace("ReOrder Medthod CalledCompleted"), Times.Once);
                mockLogger.Verify(logger => logger.LogError("ReOrder Medthod Failed to Sort List"), Times.Never);
                mockLogger.Verify(logger => logger.LogCritical("ReOrder Medthod Caused an Exception", It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
            }
        }
    }
    [TestClass]
    public class HomeControllerTests
    {
        private HomeController _homeControllerIlogger;
        private ImportTeamData _importTeamDataLogger;
        private ParseTeamStringList _parseTeamStringListLogger;
        private SortTeamsList _sortTeamsListLogger;
        [TestInitialize]
        public void TestInitialize()
        {
            // create a mock ILoggers to Mock the homecontroller            

            var mockImportTeamDataLogger = new Mock<ILogger<ImportTeamData>>();
            _importTeamDataLogger = new ImportTeamData(mockImportTeamDataLogger.Object);

            var mockParseTeamStringsLogger = new Mock<ILogger<ParseTeamStringList>>();
            _parseTeamStringListLogger = new ParseTeamStringList(mockParseTeamStringsLogger.Object);

            var mockSortTeamsListLogger = new Mock<ILogger<SortTeamsList>> ();
            _sortTeamsListLogger = new SortTeamsList(mockSortTeamsListLogger.Object);


            var mockLogger = new Mock<ILogger<HomeController>>();
            _homeControllerIlogger = new HomeController(mockLogger.Object, mockImportTeamDataLogger.Object,
                mockParseTeamStringsLogger.Object, mockSortTeamsListLogger.Object );

        }
        [TestMethod]
        public void Index_ReturnsViewWithModel()
        {
            // Act puuls the view back
            var viewResult = _homeControllerIlogger.Index() as ViewResult; 

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(HomeViewModel));

            //check to make sure the view model has the correct data
            HomeViewModel model = viewResult.Model as HomeViewModel;
            Assert.AreEqual(1, model.teamsList.Count);
            Assert.AreEqual("Away team", model.teamsList[0].awayTeamName);
            Assert.AreEqual(0, model.teamsList[0].awayTeamScore);
            Assert.AreEqual("Home team", model.teamsList[0].homeTeamName);
            Assert.AreEqual(1, model.teamsList[0].positionAdded);
        }
    }


}







