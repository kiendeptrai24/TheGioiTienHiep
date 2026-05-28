/*
 * PlayFab Unity SDK
 *
 * Copyright (c) Microsoft Corporation
 *
 * MIT License
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this
 * software and associated documentation files (the "Software"), to deal in the Software
 * without restriction, including without limitation the rights to use, copy, modify, merge,
 * publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
 * to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */

namespace PlayFab.Multiplayer.InteropWrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PFLobbyServerJoinConfiguration
    {
        public PFLobbyServerJoinConfiguration()
        {
            this.ServerProperties = new Dictionary<string, string>();
        }

        internal unsafe PFLobbyServerJoinConfiguration(Interop.PFLobbyServerJoinConfiguration interopStruct)
        {
            string[] serverPropertyKeys = Converters.StringPtrToArray(interopStruct.serverPropertyKeys, interopStruct.serverPropertyCount);
            string[] serverPropertyValues = Converters.StringPtrToArray(interopStruct.serverPropertyValues, interopStruct.serverPropertyCount);
            if (serverPropertyKeys.Length == serverPropertyValues.Length)
            {
                this.ServerProperties = Enumerable.Range(0, serverPropertyKeys.Length).ToDictionary(
                    i => serverPropertyKeys[i],
                    i => serverPropertyValues[i]);
            }
            else
            {
                throw new IndexOutOfRangeException("serverPropertyKeys and serverPropertyValues don't have same length");
            }
        }

        public IDictionary<string, string> ServerProperties { get; set; }

        internal unsafe Interop.PFLobbyServerJoinConfiguration* ToPointer(DisposableCollection disposableCollection)
        {
            Interop.PFLobbyServerJoinConfiguration interopPtr = new Interop.PFLobbyServerJoinConfiguration();

            SizeT count;
            interopPtr.serverPropertyCount = Convert.ToUInt32(this.ServerProperties.Count);
            interopPtr.serverPropertyKeys = (sbyte**)Converters.StringArrayToUTF8StringArray(this.ServerProperties.Keys.ToArray(), disposableCollection, out count);
            interopPtr.serverPropertyValues = (sbyte**)Converters.StringArrayToUTF8StringArray(this.ServerProperties.Values.ToArray(), disposableCollection, out count);

            return (Interop.PFLobbyServerJoinConfiguration*)Converters.StructToPtr<Interop.PFLobbyServerJoinConfiguration>(interopPtr, disposableCollection);
        }
    }
}
