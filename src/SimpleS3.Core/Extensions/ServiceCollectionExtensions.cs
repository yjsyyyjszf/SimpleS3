﻿using System;
using System.Collections.Generic;
using System.Reflection;
using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Fluent;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Network;
using Genbox.SimpleS3.Core.Network.RequestWrappers;
using Genbox.SimpleS3.Core.Operations;
using Genbox.SimpleS3.Core.Validation;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IValidatorFactory = Genbox.SimpleS3.Core.Abstracts.Factories.IValidatorFactory;

namespace Genbox.SimpleS3.Core.Extensions
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        public static ICoreBuilder AddSimpleS3Core(this IServiceCollection collection, Action<S3Config, IServiceProvider> configureS3)
        {
            collection?.Configure(configureS3);
            return AddSimpleS3Core(collection);
        }

        public static ICoreBuilder AddSimpleS3Core(this IServiceCollection collection, Action<S3Config> configureS3)
        {
            collection?.Configure(configureS3);
            return AddSimpleS3Core(collection);
        }

        public static ICoreBuilder AddSimpleS3Core(this IServiceCollection collection)
        {
            collection.AddLogging();
            collection.AddOptions();
            collection.TryAddSingleton<ISigningKeyBuilder, SigningKeyBuilder>();
            collection.TryAddSingleton<IScopeBuilder, ScopeBuilder>();
            collection.TryAddSingleton<ISignatureBuilder, SignatureBuilder>();
            collection.TryAddSingleton<IChunkedSignatureBuilder, ChunkedSignatureBuilder>();
            collection.TryAddSingleton<IRequestStreamWrapper, ChunkedContentRequestStreamWrapper>();
            collection.TryAddSingleton<IAuthorizationBuilder, AuthorizationHeaderBuilder>();
            collection.TryAddSingleton<IObjectOperations, ObjectOperations>();
            collection.TryAddSingleton<IBucketOperations, BucketOperations>();
            collection.TryAddSingleton<IMultipartOperations, MultipartOperations>();
            collection.TryAddSingleton<IObjectClient, S3ObjectClient>();
            collection.TryAddSingleton<IBucketClient, S3BucketClient>();
            collection.TryAddSingleton<IMultipartClient, S3MultipartClient>();
            collection.TryAddSingleton<IRequestHandler, DefaultRequestHandler>();
            collection.TryAddSingleton<IValidatorFactory, ValidatorFactory>();
            collection.TryAddSingleton<IMarshalFactory, MarshalFactory>();
            collection.TryAddSingleton<Transfer>();

            Assembly assembly = typeof(S3Config).Assembly; //Needs to be the assembly that contains the types

            collection.Add(CreateRegistrations(typeof(IValidator), assembly));
            collection.Add(CreateRegistrations(typeof(IRequestMarshal), assembly));
            collection.Add(CreateRegistrations(typeof(IResponseMarshal), assembly));

            return new CoreBuilder(collection);
        }

        private static IEnumerable<ServiceDescriptor> CreateRegistrations(Type abstractType, Assembly assembly)
        {
            foreach (Type type in TypeHelper.GetInstanceTypesInheritedFrom(abstractType, assembly))
            {
                yield return ServiceDescriptor.Singleton(abstractType, type);
            }
        }
    }
}