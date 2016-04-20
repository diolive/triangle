using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Builder
{
    public static class MapMethodExtensions
    {
        private delegate bool PathCompareDelegate(PathString path, PathString template);

        private static readonly IDictionary<PathComparison, PathCompareDelegate> pathComparisons;

        static MapMethodExtensions()
        {
            pathComparisons = new Dictionary<PathComparison, PathCompareDelegate>
            {
                [PathComparison.StartsWith] = (path, template) => path.StartsWithSegments(template),
                [PathComparison.Equals] = (path, template) => path.Equals(template),
            };
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on HTTP GET request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNet.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNet.Builder.IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder MapGet(this IApplicationBuilder app, PathString pathMatch, Action<IApplicationBuilder> configuration)
        {
            return MapGet(app, pathMatch, PathComparison.StartsWith, configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on HTTP GET request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNet.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <param name="pathComparison">The path comparison rule.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNet.Builder.IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder MapGet(this IApplicationBuilder app, PathString pathMatch, PathComparison pathComparison, Action<IApplicationBuilder> configuration)
        {
            return MapMethod(app, pathMatch, "GET", configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on HTTP POST request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNet.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNet.Builder.IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder MapPost(this IApplicationBuilder app, PathString pathMatch, Action<IApplicationBuilder> configuration)
        {
            return MapPost(app, pathMatch, PathComparison.StartsWith, configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on HTTP POST request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNet.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <param name="pathComparison">The path comparison rule.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNet.Builder.IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder MapPost(this IApplicationBuilder app, PathString pathMatch, PathComparison pathComparison, Action<IApplicationBuilder> configuration)
        {
            return MapMethod(app, pathMatch, "POST", configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on chosen HTTP method request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNet.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <param name="httpMethod">HTTP method.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNet.Builder.IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder MapMethod(this IApplicationBuilder app, PathString pathMatch, string httpMethod, Action<IApplicationBuilder> configuration)
        {
            return MapMethod(app, pathMatch, httpMethod, PathComparison.StartsWith, configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on chosen HTTP method request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNet.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <param name="httpMethod">HTTP method.</param>
        /// <param name="pathComparison">The path comparison rule.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNet.Builder.IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder MapMethod(this IApplicationBuilder app, PathString pathMatch, string httpMethod, PathComparison pathComparison, Action<IApplicationBuilder> configuration)
        {
            return app.MapWhen(context => context.Request.Method == httpMethod && pathComparisons[pathComparison](context.Request.Path, pathMatch), configuration);
        }

        public enum PathComparison
        {
            StartsWith,
            Equals,
        }
    }
}