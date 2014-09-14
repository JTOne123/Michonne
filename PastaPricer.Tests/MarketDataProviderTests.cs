﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="MarketDataProviderTests.cs" company="No lock... no deadlock" product="Michonne">
//     Copyright 2014 Cyrille DUPUYDAUBY (@Cyrdup), Thomas PIERRAIN (@tpierrain)
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
//         http://www.apache.org/licenses/LICENSE-2.0
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
//   </copyright>
//   --------------------------------------------------------------------------------------------------------------------
namespace PastaPricer.Tests
{
    using System;
    using System.Threading;
    using NFluent;
    using NUnit.Framework;

    [TestFixture]
    public class MarketDataProviderTests
    {
        private AutoResetEvent priceChangedRaisedEvent;

        #region setup/teardown

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.priceChangedRaisedEvent = new AutoResetEvent(false);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (this.priceChangedRaisedEvent != null)
            {
                this.priceChangedRaisedEvent.Dispose();
                this.priceChangedRaisedEvent = null;
            }
        }

        #endregion

        [Test]
        public void Should_provide_MarketData_for_eggs()
        {
            var marketDataProvider = new MarketDataProvider();
            marketDataProvider.RegisterStaple("eggs");

            Check.That(marketDataProvider.GetStaple("eggs")).IsInstanceOf<StapleMarketData>();
        }

        [Test]
        public void Should_return_the_same_instance_of_MarketData_given_the_same_name()
        {
            var marketDataProvider = new MarketDataProvider();
            marketDataProvider.RegisterStaple("eggs");

            Check.That(marketDataProvider.GetStaple("eggs")).IsSameReferenceThan(marketDataProvider.GetStaple("eggs"));
        }

        [Test]
        public void Should_only_get_MarketData_for_registered_assets_or_an_exception_otherwise()
        {
            var marketDataProvider = new MarketDataProvider();
            marketDataProvider.RegisterStaple("eggs");
            marketDataProvider.RegisterStaple("flour");

            Check.That(marketDataProvider.GetStaple("flour")).IsNotNull();

            Check.ThatCode(() => marketDataProvider.GetStaple("banana")).Throws<InvalidOperationException>();

            Check.That(marketDataProvider.GetStaple("eggs")).IsNotNull();
        }

        [Test]
        public void Should_receive_price_for_registered_assets_once_started()
        {
            var marketDataProvider = new MarketDataProvider();
            marketDataProvider.RegisterStaple("eggs");
            marketDataProvider.RegisterStaple("flour");
            
            marketDataProvider.GetStaple("eggs").StaplePriceChanged += (o, args) => this.priceChangedRaisedEvent.Set();

            marketDataProvider.Start();

            const int TimeoutInMsec = 100;
            var hasReceivedEvent = this.priceChangedRaisedEvent.WaitOne(TimeoutInMsec);

            Check.That(hasReceivedEvent).IsTrue();
            marketDataProvider.Stop();
        }
    }
}
