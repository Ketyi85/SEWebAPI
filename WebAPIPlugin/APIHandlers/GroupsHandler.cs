﻿using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI;
using Ingame = Sandbox.ModAPI.Ingame;

namespace WebAPIPlugin
{
    public class GroupsHandler : APIHandler
    {
        public GroupsHandler(string path) : base(path) { }

        public override void Get(HttpListenerContext context)
        {
            var uri = context.Request.Url.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped).Split('/');
            var apiBlock = APIBlockCache.Get(uri.FirstOrDefault());

            if (apiBlock == null)
            {
                return;
            }

            var gts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid((IMyCubeGrid)apiBlock.CubeGrid);
            var query = context.Request.QueryString;

            if (query.AllKeys.Contains("name"))
            {
                var bGroup = gts.GetBlockGroupWithName(query["name"]);
                if (bGroup != null)
                {
                    var resp = new List<WebTerminalBlock>();
                    foreach (var block in bGroup.Blocks)
                    {
                        resp.Add(new WebTerminalBlock(block as MyTerminalBlock));
                    }
                    context.Respond(resp, 200);
                }
                else
                {
                    context.Respond("404 Not Found", 404);
                }
            }
            else
            {
                var groups = new List<Ingame.IMyBlockGroup>();
                gts.GetBlockGroups(groups);

                var collection = new WebBlockGroupCollection();
                collection.Data = new List<WebBlockGroup>();

                foreach (var g in groups)
                {
                    collection.Data.Add(new WebBlockGroup(g));
                }

                context.Respond(collection, 200);
            }
        }

        public override void Put(HttpListenerContext context)
        {
            context.Respond("501 Not Implemented", 501);
        }
    }
}