using System;
using System.Collections.Generic;

using Microsoft.Owin;

namespace Owin
{
    public static class AppBuilderExtensions
    {
        private delegate bool PathCompareDelegate(PathString path, PathString template);

        private static readonly IDictionary<PathComparison, PathCompareDelegate> PathComparisons;

        static AppBuilderExtensions()
        {
            PathComparisons = new Dictionary<PathComparison, PathCompareDelegate>
            {
                [PathComparison.StartsWith] = (path, template) => path.StartsWithSegments(template),
                [PathComparison.Equals] = (path, template) => path.Equals(template),
            };
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on HTTP GET request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</returns>
        public static IAppBuilder MapGet(this IAppBuilder app, PathString pathMatch, Action<IAppBuilder> configuration)
        {
            return MapGet(app, pathMatch, PathComparison.StartsWith, configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on HTTP GET request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="path">The request path to match.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</returns>
        public static IAppBuilder MapGet(this IAppBuilder app, string path, Action<IAppBuilder> configuration)
        {
            return MapGet(app, new PathString(path), configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on HTTP GET request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <param name="pathComparison">The path comparison rule.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</returns>
        public static IAppBuilder MapGet(this IAppBuilder app, PathString pathMatch, PathComparison pathComparison, Action<IAppBuilder> configuration)
        {
            return MapMethod(app, pathMatch, "GET", configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on HTTP GET request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="path">The request path to match.</param>
        /// <param name="pathComparison">The path comparison rule.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</returns>
        public static IAppBuilder MapGet(this IAppBuilder app, string path, PathComparison pathComparison, Action<IAppBuilder> configuration)
        {
            return MapGet(app, new PathString(path), pathComparison, configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on HTTP POST request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</returns>
        public static IAppBuilder MapPost(this IAppBuilder app, PathString pathMatch, Action<IAppBuilder> configuration)
        {
            return MapPost(app, pathMatch, PathComparison.StartsWith, configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on HTTP POST request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="path">The request path to match.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</returns>
        public static IAppBuilder MapPost(this IAppBuilder app, string path, Action<IAppBuilder> configuration)
        {
            return MapPost(app, new PathString(path), configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on HTTP POST request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <param name="pathComparison">The path comparison rule.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</returns>
        public static IAppBuilder MapPost(this IAppBuilder app, PathString pathMatch, PathComparison pathComparison, Action<IAppBuilder> configuration)
        {
            return MapMethod(app, pathMatch, "POST", configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on HTTP POST request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="path">The request path to match.</param>
        /// <param name="pathComparison">The path comparison rule.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</returns>
        public static IAppBuilder MapPost(this IAppBuilder app, string path, PathComparison pathComparison, Action<IAppBuilder> configuration)
        {
            return MapPost(app, new PathString(path), pathComparison, configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on chosen HTTP method request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <param name="httpMethod">HTTP method.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</returns>
        public static IAppBuilder MapMethod(this IAppBuilder app, PathString pathMatch, string httpMethod, Action<IAppBuilder> configuration)
        {
            return MapMethod(app, pathMatch, httpMethod, PathComparison.StartsWith, configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on chosen HTTP method request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="path">The request path to match.</param>
        /// <param name="httpMethod">HTTP method.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</returns>
        public static IAppBuilder MapMethod(this IAppBuilder app, string path, string httpMethod, Action<IAppBuilder> configuration)
        {
            return MapMethod(app, new PathString(path), httpMethod, configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on chosen HTTP method request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <param name="httpMethod">HTTP method.</param>
        /// <param name="pathComparison">The path comparison rule.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</returns>
        public static IAppBuilder MapMethod(this IAppBuilder app, PathString pathMatch, string httpMethod, PathComparison pathComparison, Action<IAppBuilder> configuration)
        {
            return app.MapWhen(context => context.Request.Method == httpMethod && PathComparisons[pathComparison](context.Request.Path, pathMatch), configuration);
        }

        /// <summary>
        /// Branches the request pipeline based on matches of the given request path on chosen HTTP method request.
        /// If the request path starts with the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</param>
        /// <param name="path">The request path to match.</param>
        /// <param name="httpMethod">HTTP method.</param>
        /// <param name="pathComparison">The path comparison rule.</param>
        /// <param name="configuration">The branch to take for positive path matches.</param>
        /// <returns>The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> instance.</returns>
        public static IAppBuilder MapMethod(this IAppBuilder app, string path, string httpMethod, PathComparison pathComparison, Action<IAppBuilder> configuration)
        {
            return MapMethod(app, new PathString(path), httpMethod, pathComparison, configuration);
        }

        public enum PathComparison
        {
            StartsWith,
            Equals,
        }
    }
}