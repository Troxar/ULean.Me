using System.Collections.Generic;

namespace StructBenchmarking
{
    public class Experiments
    {
        public static ChartData BuildChartDataForArrayCreation(
            IBenchmark benchmark, int repetitionsCount)
        {
            var classesTimes = new List<ExperimentResult>();
            var structuresTimes = new List<ExperimentResult>();
            var factory = new ArrayCreationExperimentFactory(benchmark, repetitionsCount);

            foreach (int count in Constants.FieldCounts)
            {
                structuresTimes.Add(factory.CreateStructResult(count));
                classesTimes.Add(factory.CreateClassResult(count));
            }

            return new ChartData
            {
                Title = "Create array",
                ClassPoints = classesTimes,
                StructPoints = structuresTimes,
            };
        }

        public static ChartData BuildChartDataForMethodCall(
            IBenchmark benchmark, int repetitionsCount)
        {
            var classesTimes = new List<ExperimentResult>();
            var structuresTimes = new List<ExperimentResult>();
            var factory = new MethodCallExperimentFactory(benchmark, repetitionsCount);

            foreach (int fieldscount in Constants.FieldCounts)
            {
                structuresTimes.Add(factory.CreateStructResult(fieldscount));
                classesTimes.Add(factory.CreateClassResult(fieldscount));
            }

            return new ChartData
            {
                Title = "Call method with argument",
                ClassPoints = classesTimes,
                StructPoints = structuresTimes,
            };
        }

        private class ArrayCreationExperimentFactory : ExperimentFactory
        {
            public ArrayCreationExperimentFactory(IBenchmark benchmark, int repetitionsCount)
                : base(benchmark, repetitionsCount)
            {

            }

            public override ExperimentResult CreateStructResult(int fieldsCount)
            {
                var task = new StructArrayCreationTask(fieldsCount);
                return CreateResult(task, fieldsCount);
            }

            public override ExperimentResult CreateClassResult(int fieldsCount)
            {
                var task = new ClassArrayCreationTask(fieldsCount);
                return CreateResult(task, fieldsCount);
            }
        }

        private class MethodCallExperimentFactory : ExperimentFactory
        {
            public MethodCallExperimentFactory(IBenchmark benchmark, int repetitionsCount)
                : base(benchmark, repetitionsCount)
            {

            }

            public override ExperimentResult CreateStructResult(int fieldsCount)
            {
                var task = new MethodCallWithStructArgumentTask(fieldsCount);
                return CreateResult(task, fieldsCount);
            }

            public override ExperimentResult CreateClassResult(int fieldsCount)
            {
                var task = new MethodCallWithClassArgumentTask(fieldsCount);
                return CreateResult(task, fieldsCount);
            }
        }

        private abstract class ExperimentFactory
        {
            IBenchmark _benchmark;
            int _repetitionsCount;

            public ExperimentFactory(IBenchmark benchmark, int repetitionsCount)
            {
                _benchmark = benchmark;
                _repetitionsCount = repetitionsCount;
            }

            public abstract ExperimentResult CreateStructResult(int fieldsCount);
            public abstract ExperimentResult CreateClassResult(int fieldsCount);

            protected ExperimentResult CreateResult(ITask task, int fieldsCount)
            {
                double duration = _benchmark.MeasureDurationInMs(task, _repetitionsCount);
                return new ExperimentResult(fieldsCount, duration);
            }
        }
    }
}
