using PUBG.Stats.Core.Services.Data;
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using PUBG.Stats.Core.Services.Data.Documents.Leaderboard;
using Xunit;

namespace PUBG.Stats.Tests
{
    public class CollectionDiscoverTests
    {
        [Fact]
        public void GetCollection_WhenIsInStrategy_ThenReturnCorrectCollection()
        {
            //Arrange & Act & Assert
            CollectionDiscover.GetCollection<Leaderboard>().Should().Be("leaderboard");
        }

        [Fact]
        public void GetCollection_WhenIsNotInStrategy_ThenThrow()
        {
            //Arrange & Act & Assert
            Action act = () => CollectionDiscover.GetCollection<string>();

            act.Should().Throw<Exception>();
        }
    }
}
