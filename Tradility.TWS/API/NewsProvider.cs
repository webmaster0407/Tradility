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
     * @class NewsProvider
     * @brief Class describing news provider
     * @sa EClient::reqNewsProviders, EWrapper::newsProviders
     */
    public class NewsProvider
    {
        private string providerCode;
        private string providerName;

        /**
         * @brief The API news provider code
         */
        public string ProviderCode
        {
            get { return providerCode; }
            set { providerCode = value; }
        }

        /**
         * @brief The API news provider name
         */
        public string ProviderName
        {
            get { return providerName; }
            set { providerName = value; }
        }

        public NewsProvider()
        {

        }

        public NewsProvider(string providerCode, string providerName)
        {
            ProviderCode = providerCode;
            ProviderName = providerName;
        }
    }
}
