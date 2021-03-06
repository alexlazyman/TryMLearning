﻿using System;
using Newtonsoft.Json;
using Ninject;
using Ninject.Parameters;
using TryMLearning.Application.Interface.MachineLearning;
using TryMLearning.Application.Interface.MachineLearning.Estimates.Classifier;
using TryMLearning.Model;
using TryMLearning.Model.Constants;
using TryMLearning.Model.MachineLearning.Estimates.Classifier;

namespace TryMLearning.Application.MachineLearning
{
    public class ClassifierEstimateFactory : IClassifierEstimateFactory
    {
        private readonly IKernel _container;

        public ClassifierEstimateFactory(IKernel container)
        {
            _container = container;
        }

        public IClassifierEstimate GetEstimate(EstimateRequest estimateRequest)
        {
            if (estimateRequest == null)
            {
                throw new ArgumentNullException(nameof(estimateRequest));
            }

            var alias = estimateRequest.Alias?.ToUpper();
            var config = estimateRequest.Config;

            switch (alias)
            {
                case ClassifierEstimateAliases.Default:
                    return _container.Get<IClassifierEstimate>(alias,
                        new ConstructorArgument("config", JsonConvert.DeserializeObject<DefaultConfig>(config)));
                case ClassifierEstimateAliases.Roc:
                    return _container.Get<IClassifierEstimate>(alias,
                        new ConstructorArgument("config", JsonConvert.DeserializeObject<RocConfig>(config)));
                default:
                    throw new ArgumentException($"There is no estimate with alias: {estimateRequest.Alias}");
            }
        }
    }
}