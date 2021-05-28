﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Reflection;

namespace Quartzmin
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseQuartzmin( this IApplicationBuilder app, QuartzminOptions options, Action<Services> configure = null )
        {
            options = options ?? throw new ArgumentNullException( nameof( options ) );

            app.UseFileServer( options );

            var services = Services.Create( options );
            configure?.Invoke( services );

            app.Use( async ( context, next ) =>
             {
                 context.Items[typeof( Services )] = services;
                 await next.Invoke();
             } );
       
            app.UseEndpoints( endpoints =>
            {
                endpoints.MapControllerRoute( nameof( Quartzmin ), $"{options.VirtualPathRoot}/{{controller=Scheduler}}/{{action=Index}}" );
            } );
        }

        private static void UseFileServer( this IApplicationBuilder app, QuartzminOptions options )
        {
            IFileProvider fs;
            if ( string.IsNullOrEmpty( options.ContentRootDirectory ) )
                fs = new ManifestEmbeddedFileProvider( Assembly.GetExecutingAssembly(), "Content" );
            else
                fs = new PhysicalFileProvider( options.ContentRootDirectory );

            var fsOptions = new FileServerOptions()
            {
                RequestPath = new PathString( $"{options.VirtualPathRoot}/Content" ),
                EnableDefaultFiles = false,
                EnableDirectoryBrowsing = false,
                FileProvider = fs
            };

            app.UseFileServer( fsOptions );
        }

        public static void AddQuartzmin( this IServiceCollection services )
        {
            services.AddControllers()
                .AddApplicationPart( Assembly.GetExecutingAssembly() )
                .AddNewtonsoftJson();
        }
    }
}