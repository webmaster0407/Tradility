/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tradility.TWS.API
{
    /**
     * @class HistoricalTickBidAsk
     * @brief The historical tick's description. Used when requesting historical tick data with whatToShow = BID_ASK
     * @sa EClient, EWrapper
     */
    [ComVisible(true)]
    public class HistoricalTickBidAsk
    {
        /**
         * @brief The UNIX timestamp of the historical tick 
         */
        public long Time
        {
            [return: MarshalAs(UnmanagedType.I8)]
            get;
            [param: MarshalAs(UnmanagedType.I8)]
            private set;
        }

        /**
         * @brief Tick attribs of historical bid/ask tick
         */
        public TickAttribBidAsk TickAttribBidAsk { get; private set; }

        /**
         * @brief The bid price of the historical tick
         */
        public double PriceBid { get; private set; }

        /**
         * @brief The ask price of the historical tick 
         */
        public double PriceAsk { get; private set; }

        /**
         * @brief The bid size of the historical tick 
         */
        public long SizeBid
        {
            [return: MarshalAs(UnmanagedType.I8)]
            get;
            [param: MarshalAs(UnmanagedType.I8)]
            private set;
        }

        /**
         * @brief The ask size of the historical tick 
         */
        public long SizeAsk
        {
            [return: MarshalAs(UnmanagedType.I8)]
            get;
            [param: MarshalAs(UnmanagedType.I8)]
            private set;
        }

        public HistoricalTickBidAsk()
        {
        }

        public HistoricalTickBidAsk(long time, TickAttribBidAsk tickAttribBidAsk, double priceBid, double priceAsk, long sizeBid, long sizeAsk)
        {
            Time = time;
            TickAttribBidAsk = tickAttribBidAsk;
            PriceBid = priceBid;
            PriceAsk = priceAsk;
            SizeBid = sizeBid;
            SizeAsk = sizeAsk;
        }
    }
}
