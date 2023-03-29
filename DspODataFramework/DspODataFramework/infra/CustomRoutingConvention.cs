using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.OData.Routing.Conventions;
using System.Web.Http.OData.Routing;
using System.Web.Http.OData.Extensions;
using Microsoft.Data.Edm;

namespace DspODataFramework.infra
{
    public class CustomRoutingConvention : EntitySetRoutingConvention
    {
        public override string SelectAction(ODataPath odataPath, HttpControllerContext context,
            ILookup<string, HttpActionDescriptor> actionMap)
        {
            if (context.Request.Method == new HttpMethod("MERGE") && odataPath.PathTemplate == "~/entityset/key")
            {
                var entitySet = odataPath.Segments[0].GetEntitySet(null);

                string actionName = "Merge" + entitySet.Name;

                if (!actionMap.Contains(actionName))
                {
                    actionName = "Merge";
                }

                if (actionMap.Contains(actionName))
                {
                    // Add keys to route data, so they will bind to action parameters.
                    KeyValuePathSegment keyValueSegment = odataPath.Segments[1] as KeyValuePathSegment;
                    context.RouteData.Values[ODataRouteConstants.Key] = keyValueSegment.Value;

                    return actionName;
                }
            }
            else if (context.Request.Method == HttpMethod.Post && odataPath.PathTemplate == "~/entityset/key")
            {
                var entitySet = odataPath.Segments[0].GetEntitySet(null);

                KeyValuePathSegment keySegment = (KeyValuePathSegment)odataPath.Segments[1];
                context.RouteData.Values[ODataRouteConstants.Key] = keySegment.Value;

                string actionName = "Post" + entitySet.Name;

                if (!actionMap.Contains(actionName))
                {
                    actionName = "Post";
                }

                if (actionMap.Contains(actionName))
                {
                    return actionName;
                }
            }
            else if (context.Request.Method == HttpMethod.Get && odataPath.PathTemplate == "~/entityset/action")
            {
                var entitySet = odataPath.Segments[0].GetEntitySet(null);
                ActionPathSegment actionSegment = (ActionPathSegment)odataPath.Segments[1];
                var action = actionSegment.Action;

                if (actionMap.Contains(action.Name))
                {
                    return action.Name;
                }
            }
            else if (context.Request.Method == HttpMethod.Post && odataPath.PathTemplate == "~/entityset/action")
            {
                var entitySet = odataPath.Segments[0].GetEntitySet(null);
                ActionPathSegment actionSegment = (ActionPathSegment)odataPath.Segments[1];
                var action = actionSegment.Action;

                if (actionMap.Contains(action.Name))
                {
                    return action.Name;
                }
            }
            else if (context.Request.Method == HttpMethod.Get && odataPath.PathTemplate == "~/entityset/key/action")
            {
                var entitySet = odataPath.Segments[0].GetEntitySet(null);

                KeyValuePathSegment keySegment = (KeyValuePathSegment)odataPath.Segments[1];
                context.RouteData.Values[ODataRouteConstants.Key] = keySegment.Value;

                ActionPathSegment actionSegment = (ActionPathSegment)odataPath.Segments[2];
                var action = actionSegment.Action;

                if (actionMap.Contains(action.Name))
                {
                    return action.Name;
                }
            }
            else if (odataPath.PathTemplate.StartsWith("~/entityset/key/unresolved"))
            {
                var container = context.Request.ODataProperties().Model.EntityContainers().First();
                var segment = (UnresolvedPathSegment)odataPath.Segments[2];
                var action = segment.SegmentValue;

                if (actionMap.Contains(action))
                {
                    var entityset = odataPath.Segments[0].GetEntitySet(null);

                    EntitySetPathSegment entitysetSegment = new EntitySetPathSegment(entityset);
                    KeyValuePathSegment valuePathSegment = odataPath.Segments[1] as KeyValuePathSegment;

                    context.Request.ODataProperties().Path = new ODataPath(entitysetSegment, valuePathSegment);

                    context.RouteData.Values[ODataRouteConstants.Key] = valuePathSegment.Value;
                    //context.RouteData.Values[ODataRouteConstants.ODataPath] =;
                    return action;
                }
            }

            // Not a match.
            return base.SelectAction(odataPath, context, actionMap);
        }
    }
}
