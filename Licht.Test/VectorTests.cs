using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Globals;
using Licht.Impl.Numbers;
using NUnit.Framework;

namespace Licht.Test
{
    [TestFixture]
    public class VectorTests
    {
        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void GenericVector_Float_1Axis_MoveRight_5Amount()
        {
            GenericVector<ClassicAxis> vector = new GenericVector<ClassicAxis>(ClassicAxis.X);
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));

            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.X, 1.0f));
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(5f));
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void GenericVector_Float_1Axis_MoveLeft_5Amount()
        {
            GenericVector<ClassicAxis> vector = new GenericVector<ClassicAxis>(ClassicAxis.X);
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));

            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.X, -1.0f));
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(-5f));
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void GenericVector_Float_1Axis_MoveRight_5Amount_MoveLeft_5Amount()
        {
            GenericVector<ClassicAxis> vector = new GenericVector<ClassicAxis>(ClassicAxis.X);
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));

            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.X, 1.0f),
                new DirectionVector<float, ClassicAxis>(ClassicAxis.X, -1.0f));

            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void GenericVector_Float_1Axis_MoveRight_5Amount_MoveLeft_5Amount_Separate()
        {
            GenericVector<ClassicAxis> vector = new GenericVector<ClassicAxis>(ClassicAxis.X);
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));

            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.X, 1.0f));
            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.X, -1.0f));

            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void GenericVector_Float_2Axis_MoveRight_5Amount()
        {
            GenericVector<ClassicAxis> vector = new GenericVector<ClassicAxis>(ClassicAxis.X, ClassicAxis.Y);
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq(0));

            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.X, 1.0f));
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(5f));
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void GenericVector_Float_2Axis_MoveUp_5Amount()
        {
            GenericVector<ClassicAxis> vector = new GenericVector<ClassicAxis>(ClassicAxis.X, ClassicAxis.Y);
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq(0));

            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.Y, 1.0f));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq(5f));
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void GenericVector_Float_2Axis_MoveUpRight_5Amount()
        {
            GenericVector<ClassicAxis> vector = new GenericVector<ClassicAxis>(ClassicAxis.X, ClassicAxis.Y);
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq(0));

            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.X, 1.0f),
                new DirectionVector<float, ClassicAxis>(ClassicAxis.Y, 1.0f));

            Assert.IsTrue(vector[ClassicAxis.X].FloatEq((float)(5 * Math.Sqrt(2) / 2)));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq((float)(5 * Math.Sqrt(2) / 2)));
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void GenericVector_Float_2Axis_MoveDownRight_5Amount()
        {
            GenericVector<ClassicAxis> vector = new GenericVector<ClassicAxis>(ClassicAxis.X, ClassicAxis.Y);
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq(0));

            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.X, 1.0f),
                new DirectionVector<float, ClassicAxis>(ClassicAxis.Y, -1.0f));

            Assert.IsTrue(vector[ClassicAxis.X].FloatEq((float)(5 * Math.Sqrt(2) / 2)));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq((float)(-5 * Math.Sqrt(2) / 2)));
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void GenericVector_Float_2Axis_MoveUpLeft_5Amount()
        {
            GenericVector<ClassicAxis> vector = new GenericVector<ClassicAxis>(ClassicAxis.X, ClassicAxis.Y);
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq(0));

            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.X, -1.0f),
                new DirectionVector<float, ClassicAxis>(ClassicAxis.Y, 1.0f));

            Assert.IsTrue(vector[ClassicAxis.X].FloatEq((float)(-5 * Math.Sqrt(2) / 2)));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq((float)(5 * Math.Sqrt(2) / 2)));
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void GenericVector_Float_2Axis_MoveDownLeft_5Amount()
        {
            GenericVector<ClassicAxis> vector = new GenericVector<ClassicAxis>(ClassicAxis.X, ClassicAxis.Y);
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq(0));

            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.X, -1.0f),
                new DirectionVector<float, ClassicAxis>(ClassicAxis.Y, -1.0f));

            Assert.IsTrue(vector[ClassicAxis.X].FloatEq((float)(-5 * Math.Sqrt(2) / 2)));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq((float)(-5 * Math.Sqrt(2) / 2)));
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void GenericVector_Float_3Axis_MoveRight_5Amount()
        {
            GenericVector<ClassicAxis> vector = new GenericVector<ClassicAxis>(ClassicAxis.X, ClassicAxis.Y, ClassicAxis.Z);

            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq(0));
            Assert.IsTrue(vector[ClassicAxis.Z].FloatEq(0));

            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.X, 1.0f));
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(5f));
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void GenericVector_Float_3Axis_MoveUpRightForward_5Amount()
        {
            GenericVector<ClassicAxis> vector = new GenericVector<ClassicAxis>(ClassicAxis.X, ClassicAxis.Y, ClassicAxis.Z);
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq(0));
            Assert.IsTrue(vector[ClassicAxis.Z].FloatEq(0));

            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.X, 1.0f),
                new DirectionVector<float, ClassicAxis>(ClassicAxis.Y, 1.0f),
                new DirectionVector<float, ClassicAxis>(ClassicAxis.Z, 1.0f));


            Assert.IsTrue(vector[ClassicAxis.X].FloatEq((float)(5 * Math.Sqrt(3) / 3)));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq((float)(5 * Math.Sqrt(3) / 3)));
            Assert.IsTrue(vector[ClassicAxis.Z].FloatEq((float)(5 * Math.Sqrt(3) / 3)));
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void GenericVector_Float_3Axis_MoveUpRightBackward_5Amount()
        {
            GenericVector<ClassicAxis> vector = new GenericVector<ClassicAxis>(ClassicAxis.X, ClassicAxis.Y, ClassicAxis.Z);
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq(0));
            Assert.IsTrue(vector[ClassicAxis.Z].FloatEq(0));

            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.X, 1.0f),
                new DirectionVector<float, ClassicAxis>(ClassicAxis.Y, 1.0f),
                new DirectionVector<float, ClassicAxis>(ClassicAxis.Z, -1.0f));

            Assert.IsTrue(vector[ClassicAxis.X].FloatEq((float)(5 * Math.Sqrt(3) / 3)));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq((float)(5 * Math.Sqrt(3) / 3)));
            Assert.IsTrue(vector[ClassicAxis.Z].FloatEq((float)(-5 * Math.Sqrt(3) / 3)));
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void GenericVector_Float_3Axis_MoveUpRight_5Amount_Forward_2Amount()
        {
            GenericVector<ClassicAxis> vector = new GenericVector<ClassicAxis>(ClassicAxis.X, ClassicAxis.Y, ClassicAxis.Z);
            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(0));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq(0));
            Assert.IsTrue(vector[ClassicAxis.Z].FloatEq(0));

            vector.Translate(5f, new DirectionVector<float, ClassicAxis>(ClassicAxis.X, 1.0f),
                new DirectionVector<float, ClassicAxis>(ClassicAxis.Y, 1f),
                new DirectionVector<float, ClassicAxis>(ClassicAxis.Z, 0.4f));

            Assert.IsTrue(vector[ClassicAxis.X].FloatEq(3.402f));
            Assert.IsTrue(vector[ClassicAxis.Y].FloatEq(3.402f));
            Assert.IsTrue(vector[ClassicAxis.Z].FloatEq(1.361f));
        }
    }
}
