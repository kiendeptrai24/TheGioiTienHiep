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
    using System.Runtime.InteropServices;
    using PlayFab.Multiplayer.Interop;

    public class PFLobbyServerDataUpdate
    {
        public PFLobbyServerDataUpdate()
        {
            this.ServerProperties = new Dictionary<string, string>();
        }

        internal unsafe PFLobbyServerDataUpdate(Interop.PFLobbyServerDataUpdate interopStruct)
        {
            this.NewServer = new PFEntityKey(interopStruct.newServer);

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

        public PFEntityKey NewServer { get; set; }

        public IDictionary<string, string> ServerProperties { get; set; }

        internal unsafe Interop.PFLobbyServerDataUpdate* ToPointer(DisposableCollection disposableCollection)
        {
            Interop.PFLobbyServerDataUpdate interopPtr = new Interop.PFLobbyServerDataUpdate();

            interopPtr.newServer = this.NewServer != null ? this.NewServer.ToPointer(disposableCollection) : null;

            SizeT count;
            interopPtr.serverPropertyCount = this.ServerProperties != null ? Convert.ToUInt32(this.ServerProperties.Count) : 0;
            interopPtr.serverPropertyKeys = interopPtr.serverPropertyCount > 0 ? (sbyte**)Converters.StringArrayToUTF8StringArray(this.ServerProperties.Keys.ToArray(), disposableCollection, out count) : null;
            interopPtr.serverPropertyValues = interopPtr.serverPropertyCount > 0 ? (sbyte**)Converters.StringArrayToUTF8StringArray(this.ServerProperties.Values.ToArray(), disposableCollection, out count) : null;

            return (Interop.PFLobbyServerDataUpdate*)Converters.StructToPtr<Interop.PFLobbyServerDataUpdate>(interopPtr, disposableCollection);
        }
    }
}
