﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using TryMLearning.Model;
using TryMLearning.Persistence.Models;

namespace TryMLearning.Persistence.Configuration
{
    public static class AutoMapperConfiguration
    {
        public static void RegisterDtoMaps(this IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<User, UserDbEntity>();
            cfg.CreateMap<UserDbEntity, User>();

            cfg.CreateMap<ClassAlias, ClassAliasDbEntity>();
            cfg.CreateMap<ClassAliasDbEntity, ClassAlias>();

            cfg.CreateMap<Algorithm, AlgorithmDbEntity>()
                .ForMember(sdb => sdb.AlgorithmParameters, opt => opt.Ignore())
                .ForMember(sdb => sdb.Author, opt => opt.Ignore())
                .ForMember(sdb => sdb.AuthorId, opt => opt.MapFrom(s => s.Author.UserId));
            cfg.CreateMap<AlgorithmDbEntity, Algorithm>()
                .ForMember(s => s.Parameters, opt => opt.MapFrom(sdb => sdb.AlgorithmParameters));

            cfg.CreateMap<AlgorithmParameter, AlgorithmParameterDbEntity>();
            cfg.CreateMap<AlgorithmParameterDbEntity, AlgorithmParameter>();

            cfg.CreateMap<AlgorithmParameterValue, AlgorithmParameterValueDbEntity>();
            cfg.CreateMap<AlgorithmParameterValueDbEntity, AlgorithmParameterValue>();

            cfg.CreateMap<Estimation, EstimationDbEntity>()
                .ForMember(sdb => sdb.User, opt => opt.Ignore())
                .ForMember(sdb => sdb.UserId, opt => opt.MapFrom(s => s.User.UserId))
                .ForMember(sdb => sdb.AlgorithmParameterValues, opt => opt.Ignore())
                .ForMember(sdb => sdb.Algorithm, opt => opt.Ignore())
                .ForMember(sdb => sdb.AlgorithmId, opt => opt.ResolveUsing(s => s.Algorithm.AlgorithmId))
                .ForMember(sdb => sdb.DataSet, opt => opt.Ignore())
                .ForMember(sdb => sdb.DataSetId, opt => opt.ResolveUsing(s => s.DataSet.DataSetId));
            cfg.CreateMap<EstimationDbEntity, Estimation>()
                .ForMember(s => s.ParameterValues, opt => opt.MapFrom(sdb => sdb.AlgorithmParameterValues))
                .ForMember(s => s.Algorithm, opt => opt.ResolveUsing(sdb => sdb.Algorithm ?? (object) new Algorithm(sdb.AlgorithmId)))
                .ForMember(s => s.DataSet, opt => opt.ResolveUsing(sdb => sdb.DataSet ?? (object) new DataSet(sdb.DataSetId)));

            cfg.CreateMap<DataSet, DataSetDbEntity>()
                .ForMember(sdb => sdb.Author, opt => opt.Ignore())
                .ForMember(sdb => sdb.AuthorId, opt => opt.MapFrom(s => s.Author.UserId));
            cfg.CreateMap<DataSetDbEntity, DataSet>();

            cfg.CreateMap<ClassificationSample, ClassificationSampleDbEntity>()
                .ForMember(sdb => sdb.Count, opt => opt.ResolveUsing(s => s.Features?.Length ?? 0))
                .ForMember(sdb => sdb.FeatureTuples, opt => opt.ResolveUsing(s => ToDoubleTuples(s.Features)));
            cfg.CreateMap<ClassificationSampleDbEntity, ClassificationSample>()
                .ForMember(s => s.Features, opt => opt.ResolveUsing(s => FromDoubleTuples(s.Count, s.FeatureTuples)));

            cfg.CreateMap<ClassificationResult, ClassificationResultDbEntity>();
            cfg.CreateMap<ClassificationResultDbEntity, ClassificationResult>();
        }

        private static List<DoubleTupleDbEntity> ToDoubleTuples(double[] values)
        {
            if (values == null)
            {
                return null;
            }

            var tupleCount = Math.Ceiling((double)values.Length / DoubleTupleDbEntity.Capacity);
            IEnumerable<double> valueEnumerable = values;

            var tuples = new List<DoubleTupleDbEntity>();
            for (int i = 0; i < tupleCount; i++)
            {
                tuples.Add(new DoubleTupleDbEntity(valueEnumerable, i));
                valueEnumerable = valueEnumerable.Skip(DoubleTupleDbEntity.Capacity);
            }

            return tuples;
        }

        private static double[] FromDoubleTuples(int count, IEnumerable<DoubleTupleDbEntity> tuples)
        {
            if (tuples == null)
            {
                return null;
            }

            var result = new double[count];

            var i = 0;
            foreach (var doubleTuple in tuples.OrderBy(m => m.Order))
            {
                foreach (var doubleValue in doubleTuple)
                {
                    if (i >= count)
                    {
                        break;
                    }

                    result[i++] = doubleValue.Value;
                }
            }

            return result;
        }
    }
}