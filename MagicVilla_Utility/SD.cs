﻿namespace MagicVilla_Utility
{
    public class SD
    {
        public enum ApiType
        {
            GET,
            POST, 
            PUT, 
            DELETE
        }
        public static string SessionToken = "JWTToken";
        public static string CurrentAPIVersion = "v2";
    }
}
