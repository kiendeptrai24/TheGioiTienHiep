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

namespace PlayFab.Multiplayer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The initial configuration data used when joining a client-owned lobby as a server.
    /// </summary>
    public class LobbyServerJoinConfiguration
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        public LobbyServerJoinConfiguration()
        {
            this.Config = new InteropWrapper.PFLobbyServerJoinConfiguration();
        }

        /// <summary>
        /// The initial properties for the server joining the lobby.
        /// </summary>
        public IDictionary<string, string> ServerProperties
        {
            get
            {
                return this.Config.ServerProperties;
            }

            set
            {
                this.Config.ServerProperties = value;
            }
        }

        internal InteropWrapper.PFLobbyServerJoinConfiguration Config { get; set; }
    }
}
