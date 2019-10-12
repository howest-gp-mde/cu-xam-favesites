using FluentValidation;
using Moq;
using System;
using System.Linq;
using XrnCourse.FavoriteSites.Domain.Models;
using XrnCourse.FavoriteSites.Domain.Services;
using Xunit;

namespace XrnCourse.FavoriteSites.Tests
{
    public class SiteServiceTests
    {
        Site[] testSites;

        public SiteServiceTests()
        {
            testSites = TestData.TestSites;
        }


        [Fact]
        public void Delete_Returns_DeletedSiteInstance()
        {
            //arrange
            var testSite = testSites[0];

            var mockSiteRepo = new Mock<ISiteRepository>();
            mockSiteRepo.Setup(repo => repo.DeleteSite(testSite.Id))
                .Returns(testSite); //repo returns the "fake deleted" site successfully

            var siteService = new SiteService(mockSiteRepo.Object);

            //act
            var deletedSite = siteService.Delete(testSite.Id);

            //assert
            Assert.NotNull(deletedSite);
            Assert.Equal(testSite.Id, deletedSite.Id);
        }

        [Fact]
        public void GetAll_Returns_SortedResults()
        {
            //arrange
            var mockSiteRepo = new Mock<ISiteRepository>();
            mockSiteRepo.Setup(repo => repo.GetSites())
                .Returns(testSites); //repo returns unordered list to test

            var expectedResults = testSites
                .OrderByDescending(s => s.TimesVisited)
                .ThenBy(s => s.Name);

            var siteService = new SiteService(mockSiteRepo.Object);

            //act
            var actualResults = siteService.GetAll();


            //assert
            var expectedSites = expectedResults.ToArray();  //helps iterating over collection
            var actualSites = actualResults.ToArray();      //helps iterating over collection

            // -> resulting sites should be ordered by number of visits first
            for (int i = 0; i < expectedSites.Length; i++)
            {
                Assert.Equal(expectedSites[i].Id, actualSites[i].Id);
            }
        }


        [Fact]
        public void Save_PrefixesScheme_When_UrlHasNoScheme()
        {
            //arrange
            var testSite = TestData.InvalidPrefixedSites[0];
            var mockSiteRepo = new Mock<ISiteRepository>();
            
            //configure fake AddSite method
            mockSiteRepo
                .Setup(e => e.AddSite(It.IsAny<Site>()))
                .Returns<Site>(inputSite => inputSite); //returns same site as input (It)
            //configure fake UpdateSite method
            mockSiteRepo
                .Setup(e => e.UpdateSite(It.IsAny<Site>()))
                .Returns<Site>(inputSite => inputSite); //returns same site as input (It)

            var siteService = new SiteService(mockSiteRepo.Object);

            //act
            var result = siteService.Save(testSite);

            //assert
            Assert.StartsWith("https://", result.Url);
        }

        [Fact]
        public void Save_Updates_When_SiteIdExists()
        {
            //arrange
            var mockSiteRepo = new Mock<ISiteRepository>();
            mockSiteRepo
                .Setup(repo => repo.GetSite(It.IsAny<Guid>()))
                .Returns<Guid>(id => testSites.FirstOrDefault(e => e.Id == id)); //repo returns unordered list to test
            mockSiteRepo
                .Setup(e => e.AddSite(It.IsAny<Site>()))
                .Returns<Site>(inputSite => inputSite); //returns same site as input (It)
            mockSiteRepo
                .Setup(e => e.UpdateSite(It.IsAny<Site>()))
                .Returns<Site>(inputSite => inputSite); //returns same site as input (It)
            
            var siteService = new SiteService(mockSiteRepo.Object);

            var siteToUpdate = new Site
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), //existing Id
                Name = "Updated Site",
                Rating = 5,
                TimesVisited = 0,
                Url = "https://www.github.com"
            };

            //act
            var updatedSite = siteService.Save(siteToUpdate);

            //assert
            mockSiteRepo.Verify(m => m.UpdateSite(siteToUpdate), Times.AtLeastOnce());
        }

        [Fact]
        public void Save_Inserts_When_SiteIdDoesntExist()
        {
            //arrange
            var mockSiteRepo = new Mock<ISiteRepository>();
            mockSiteRepo
                .Setup(repo => repo.GetSite(It.IsAny<Guid>()))
                .Returns<Guid>(id => testSites.FirstOrDefault(e => e.Id == id)); //repo returns unordered list to test
            mockSiteRepo
                .Setup(e => e.AddSite(It.IsAny<Site>()))
                .Returns<Site>(inputSite => inputSite); //returns same site as input (It)
            mockSiteRepo
                .Setup(e => e.UpdateSite(It.IsAny<Site>()))
                .Returns<Site>(inputSite => inputSite); //returns same site as input (It)

            var siteService = new SiteService(mockSiteRepo.Object);

            var siteToInsert = new Site
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), //non-existing Id
                Name = "Inserted Site",
                Rating = 5,
                TimesVisited = 0,
                Url = "https://www.github.com"
            };

            //act
            var insertedSite = siteService.Save(siteToInsert);

            //assert
            mockSiteRepo.Verify(m => m.AddSite(siteToInsert), Times.AtLeastOnce());
        }


        [Fact]
        public void Save_Throws_ValidationException_When_NameNotEmpty()
        {
            //arrange
            var siteService = new SiteService(null);
            var invalidSite = new Site
            {
                Id = Guid.NewGuid(),
                Name = null,
                Rating = 2,
                TimesVisited = 0,
                Url = "https://www.github.com"
            };

            //act
            var saveAction = new Action(() => { siteService.Save(invalidSite); });
            
            //assert
            Assert.Throws<ValidationException>(saveAction);
        }


        [Fact]
        public void Save_Throws_ValidationException_When_NameOutOfRange()
        {
            //arrange
            var siteService = new SiteService(null);
            var invalidSiteOne = new Site
            {
                Id = Guid.NewGuid(),
                Name = "a",
                Rating = 2,
                TimesVisited = 0,
                Url = "https://www.github.com"
            };
            var invalidSiteTwo = new Site
            {
                Id = Guid.NewGuid(),
                Name = "aaaaaaaaaabbbbbbbbbbccccccccccd", //31 chars
                Rating = 2,
                TimesVisited = 0,
                Url = "https://www.github.com"
            };

            //act
            var saveActionOne = new Action(() => { siteService.Save(invalidSiteOne); });
            var saveActionTwo = new Action(() => { siteService.Save(invalidSiteTwo); });

            //assert
            Assert.Throws<ValidationException>(saveActionOne);
            Assert.Throws<ValidationException>(saveActionTwo);
        }

        [Fact]
        public void Save_Throws_ValidationException_When_InvalidUrl()
        {
            //arrange
            var siteService = new SiteService(null);
            var invalidSite = new Site
            {
                Id = Guid.NewGuid(),
                Name = "Valid Name",
                Rating = 2,
                TimesVisited = 0,
                Url = "invalid url"
            };

            //act
            var saveAction = new Action(() => { siteService.Save(invalidSite); });

            //assert
            Assert.Throws<ValidationException>(saveAction);
        }

        [Fact]
        public async void Open_Increments_TimesVisited()
        {
            //arrange
            var testSite = testSites[0];
            var expectedVisits = testSite.TimesVisited + 1;

            var mockSiteRepo = new Mock<ISiteRepository>();
            mockSiteRepo.Setup(repo => repo.GetSite(testSite.Id))
                .Returns(testSite);

            var siteService = new SiteService(mockSiteRepo.Object);
            
            //act
            await siteService.Open(testSite.Id);
            var site = siteService.Get(testSite.Id); //get site for assertion

            //assert
            Assert.Equal(expectedVisits, site.TimesVisited);
        }
    }
}
