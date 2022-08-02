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
     * @class FamilyCode
     * @brief Class describing family code
     * @sa EClient::reqFamilyCodes, EWrapper::familyCodes
     */
    public class FamilyCode
    {
        private string accountID;
        private string familyCodeStr;

        /**
         * @brief The API account id
         */
        public string AccountID
        {
            get { return accountID; }
            set { accountID = value; }
        }

        /**
         * @brief The API family code
         */
        public string FamilyCodeStr
        {
            get { return familyCodeStr; }
            set { familyCodeStr = value; }
        }

        public FamilyCode()
        {

        }

        public FamilyCode(string accountID, string familyCodeStr)
        {
            AccountID = accountID;
            FamilyCodeStr = familyCodeStr;
        }
    }
}
