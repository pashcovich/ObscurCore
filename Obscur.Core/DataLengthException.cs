﻿//
//  Copyright 2013  Matthew Ducker
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Runtime.Serialization;

namespace Obscur.Core
{
    /// <summary>
    ///     Exception thrown when a data source, destination, or buffer 
    ///     is either too short or too long.
    /// </summary>
    [Serializable]
    public class DataLengthException : Exception
    {
        private const string ExceptionMessage = "Data is incorrect length.";

        public DataLengthException() : base(ExceptionMessage) {}
        public DataLengthException(string message) : base(message) {}
        public DataLengthException(string message, Exception inner) : base(message, inner) {}

        public DataLengthException(string message, string parameter)
            : base(message + "\nParameter '" + parameter + "' is at fault.") {}

        protected DataLengthException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {}
    }
}
