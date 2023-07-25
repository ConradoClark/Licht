using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Generation;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Generation
{
    public abstract class Grid2DMapGenerator<TObject> : BaseGameObject where TObject: ProcGenObject
    {
        [field:SerializeField]
        public Grid2DMapMemory MapMemory { get; private set; }
        [field:SerializeField]
        public Grid2DRegion<TObject>[] Regions { get;private set; }
        private Grid2DPositionalGenerator _positionalGenerator;
        private WeightedDice<TObject> _diceRoller;
        
        protected override void OnAwake()
        {
            base.OnAwake();
            _diceRoller = new WeightedDice<TObject>(Array.Empty<TObject>(), _positionalGenerator);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _positionalGenerator = new Grid2DPositionalGenerator(Environment.TickCount);
        }

        private IProcGenRegion<Vector2Int, TObject> GetRegion(Vector2Int position)
        {
            return Regions.FirstOrDefault(region => region.IsInRegion(position));
        }
        
        public IEnumerable<TObject> Generate(Vector2Int position)
        {
            _positionalGenerator.Position = position;
            var region = GetRegion(position);

            if (region == null) yield break;
            
            foreach (var stack in region.PossibleObjectStacks)
            {
                _diceRoller.SetPossibleValues(stack);
                yield return _diceRoller.Generate();
            }
        }
    }
}