/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tradility.TWS.API
{
    /**
     * @class DepthMktDataDescription
     * @brief A class for storing depth market data desctiption
     */
    public class DepthMktDataDescription
    {
        private string exchange;
        private string secType;
        private string listingExch;
        private string serviceDataType;
        private int aggGroup;

        /**
         * @brief The exchange name
         */
        public string Exchange
        {
            get { return exchange; }
            set { exchange = value; }
        }

        /**
         * @brief The security type
         */
        public string SecType
        {
            get { return secType; }
            set { secType = value; }
        }

        /**
         * @brief The listing exchange name
         */
        public string ListingExch
        {
            get { return listingExch; }
            set { listingExch = value; }
        }

        /**
         * @brief The service data type
         */
        public string ServiceDataType
        {
            get { return serviceDataType; }
            set { serviceDataType = value; }
        }

        /**
         * @brief The aggregated group
         */
        public int AggGroup
        {
            get { return aggGroup; }
            set { aggGroup = value; }
        }

        public DepthMktDataDescription()
        {
        }

        public DepthMktDataDescription(string exchange, string secType, string listingExch, string serviceDataType, int aggGroup)
        {
            Exchange = exchange;
            SecType = secType;
            ListingExch = listingExch;
            ServiceDataType = serviceDataType;
            AggGroup = aggGroup;
        }
    }
}
