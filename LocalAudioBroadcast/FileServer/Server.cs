﻿// Copyright 2014 Mario Guggenberger <mg@protyposis.net>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace LocalAudioBroadcast.FileServer {
    class Server {

        private HttpServer.HttpServer httpServer;
        private List<ServerModule> httpModules;
        public event EventHandler ServerSocketClosed;
        public Server() {
            httpModules = new List<ServerModule>();
        }

        protected virtual void OnSocketClosed(EventArgs e)
        {
            ServerSocketClosed?.Invoke(this, e);
        }

        public void Add(ServerModule module) {
            httpModules.Add(module);
            Console.WriteLine("HTTP module added: " + module.GetType().Name);
        }

        public void Start(IPEndPoint ipEndPoint) {

            httpServer = new HttpServer.HttpServer();
            httpServer.Start(ipEndPoint.Address, ipEndPoint.Port);

            httpModules.ForEach(module => { module.Start(); httpServer.Add(module); ((LoopbackModule)module).socketClosed += Server_socketClosed; });

            Console.WriteLine("HTTP server STARTED listening @ " + ipEndPoint);
        }

        private void Server_socketClosed(object sender, EventArgs e)
        {
            OnSocketClosed(EventArgs.Empty);
        }

        public void Stop() {
            httpModules.ForEach(module => { module.Stop(); ((LoopbackModule)module).socketClosed -= Server_socketClosed; });
            httpServer.Stop();
            Console.WriteLine("HTTP server STOPPED");
        }
    }
}
