using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using NUnit.Framework;

namespace Licht.Test
{
    [TestFixture]
    public class OrchestrationTests
    {
        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void Machinery_StringBuilder_12345_5Machines()
        {
            StringBuilder builder = new StringBuilder();
            var machinery = new BasicMachinery();
            machinery.AddMachines(new BasicMachine(1, ()=> builder.Append("a")),
                new BasicMachine(2, () => builder.Append("b")),
                new BasicMachine(3, () => builder.Append("c")),
                new BasicMachine(4, () => builder.Append("d")),
                new BasicMachine(5, () => builder.Append("e"))
            );
            machinery.Update();
            Assert.AreEqual("abcde", builder.ToString());

            machinery.Update();
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void Machinery_StringBuilder_12345_1Machine()
        {
            StringBuilder builder = new StringBuilder();
            var machinery = new BasicMachinery();
            machinery.AddMachines(new BasicMachine(1,new Action[]
            {
                () => builder.Append("a"),
                () => builder.Append("b"),
                () => builder.Append("c"),
                () => builder.Append("d"),
                () => builder.Append("e"),
            })
            );
            machinery.Update();
            machinery.Update();
            machinery.Update();
            machinery.Update();
            machinery.Update();
            Assert.AreEqual("abcde", builder.ToString());
            
            machinery.Update();
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void Machinery_StringBuilder_13254_5Machines()
        {
            StringBuilder builder = new StringBuilder();
            var machinery = new BasicMachinery();
            machinery.AddMachines(new BasicMachine(1, () => builder.Append("a")),
                new BasicMachine(3, () => builder.Append("b")),
                new BasicMachine(2, () => builder.Append("c")),
                new BasicMachine(5, () => builder.Append("d")),
                new BasicMachine(4, () => builder.Append("e"))
            );
            machinery.Update();
            Assert.AreEqual("acbed", builder.ToString());

            machinery.Update();
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void Machinery_StringBuilder_12_2Machines_Queued_FIFO()
        {
            StringBuilder builder = new StringBuilder();
            var machinery = new BasicMachinery();

            var bq = new FIFOQueue();
            machinery.AddMachinesWithQueue(bq, 
                new BasicMachine(1, () => builder.Append("a")),
                new BasicMachine(1, () => builder.Append("b")));

            machinery.Update();
            Assert.AreEqual("a", builder.ToString());

            machinery.Update();
            Assert.AreEqual("ab", builder.ToString());

            machinery.Update();
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void Machinery_StringBuilder_21_2Machines_Queued_Priority()
        {
            StringBuilder builder = new StringBuilder();
            var machinery = new BasicMachinery();

            var pq = new PriorityQueue();
            machinery.AddMachinesWithQueue(pq,
                new BasicMachine(2, () => builder.Append("a")),
                new BasicMachine(1, () => builder.Append("b")));

            machinery.Update();
            Assert.AreEqual("b", builder.ToString());

            machinery.Update();
            Assert.AreEqual("ba", builder.ToString());

            machinery.Update();
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void Machinery_StringBuilder_123_2Machines_Queued_Priority_1Machine_NoQueue()
        {
            StringBuilder builder = new StringBuilder();
            var machinery = new BasicMachinery();

            var pq = new PriorityQueue();
            machinery.AddMachinesWithQueue(pq,
                new BasicMachine(2, () => builder.Append("b")),
                new BasicMachine(3, () => builder.Append("c")));

            machinery.AddMachines(new BasicMachine(1, ()=> builder.Append("a")));

            machinery.Update();
            Assert.AreEqual("ab", builder.ToString());

            machinery.Update();
            Assert.AreEqual("abc", builder.ToString());

            machinery.Update();
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void Machinery_StringBuilder_12_2Machines_WaitForCondition()
        {
            StringBuilder builder = new StringBuilder();
            var machinery = new BasicMachinery();

            int test = 0;

            var pq = new PriorityQueue();
            machinery.AddMachinesWithQueue(pq,
                new WaitForConditionMachine(1, () => test == 1),
                new BasicMachine(2, () => builder.Append("abc")));

            machinery.Update();
            machinery.Update();
            machinery.Update();
            machinery.Update();

            test = 1;
            machinery.Update();

            Assert.AreEqual("abc", builder.ToString());
        }
    }
}
