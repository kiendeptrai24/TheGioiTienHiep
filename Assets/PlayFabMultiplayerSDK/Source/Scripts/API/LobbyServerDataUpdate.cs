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
    /// A request to make an update to the associated server state of a client-owned lobby.
    /// </summary>
    public class LobbyServerDataUpdate
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        public LobbyServerDataUpdate()
        {
            this.Update = new InteropWrapper.PFLobbyServerDataUpdate();
        }

        /// <summary>
        /// An optional, new server to be associated with the client-owned lobby.
        /// </summary>
        /// <remarks>
        /// If specified, this entity must be a game_server entity.
        /// <para>
        /// There can only be one server associated with a lobby at a time. Setting a new server here replaces the currently
        /// associated server in the lobby.
        /// </para>
        /// </remarks>
        public PFEntityKey NewServer
        {
            get
            {
                return new PFEntityKey(this.Update.NewServer);
            }

            set
            {
                this.Update.NewServer = value.EntityKey;
            }
        }

        /// <summary>
        /// The server properties to update.
        /// </summary>
        /// <remarks>
        /// There might only be <c>PFLobbyMaxServerPropertyCount</c> concurrent properties at any given time. Therefore, at
        /// most, twice that many unique properties can be specified in this update if half of those properties are being
        /// deleted.
        /// <para>
        /// If the property limits are violated, the entire update operation fails.
        /// </para>
        /// </remarks>
        public IDictionary<string, string> SearchProperties
        {
            get
            {
                return this.Update.ServerProperties;
            }

            set
            {
                this.Update.ServerProperties = value;
            }
        }

        internal InteropWrapper.PFLobbyServerDataUpdate Update { get; set; }
    }
}
