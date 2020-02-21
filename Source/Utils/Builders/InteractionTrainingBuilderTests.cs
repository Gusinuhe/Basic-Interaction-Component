﻿using System;
using System.Collections;
using System.Linq;
using Innoactive.Hub.Training;
using Innoactive.Hub.Training.Conditions;
using Innoactive.Hub.Training.Configuration.Modes;
using Innoactive.Hub.Training.SceneObjects;
using Innoactive.Hub.Training.SceneObjects.Interaction.Properties;
using Innoactive.Hub.Training.SceneObjects.Properties;
using Innoactive.Hub.Training.Utils.Builders;
using Innoactive.Hub.Training.Utils.Interaction.Builders;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Innoactive.Hub.Unity.Tests.Training.Interaction
{
    public class InteractionTrainingBuilderTests : RuntimeTests
    {
        public class DummySnapZoneProperty : LockableProperty, ISnapZoneProperty
        {
            protected override void InternalSetLocked(bool lockState)
            {
                throw new NotImplementedException();
            }

            public event EventHandler<EventArgs> ObjectSnapped;
            public event EventHandler<EventArgs> ObjectUnsnapped;
            
            public bool IsObjectSnapped { get; }
            
            public ISnappableProperty SnappedObject { get; set; }
            
            public GameObject SnapZoneObject { get; }
            
            public void Configure(IMode mode)
            {
                throw new NotImplementedException();
            }
        }
        
        public class DummySnappableProperty : TrainingSceneObjectProperty, ISnappableProperty
        {
            public event EventHandler<EventArgs> Snapped;
            public event EventHandler<EventArgs> Unsnapped;
            
            public bool IsSnapped { get; }
            
            public bool LockObjectOnSnap { get; }
            
            public ISnapZoneProperty SnappedZone { get; set; }
            
            public void FastForwardSnapInto(ISnapZoneProperty snapZone)
            {
                throw new NotImplementedException();
            }
        }

        public class DummyUsableProperty : LockableProperty, IUsableProperty
        {
            protected override void InternalSetLocked(bool lockState)
            {
                throw new NotImplementedException();
            }

            public event EventHandler<EventArgs> UsageStarted;
            public event EventHandler<EventArgs> UsageStopped;
            
            public bool IsBeingUsed { get; }
            
            public void FastForwardUse()
            {
                throw new NotImplementedException();
            }
        }

        public class DummyTouchableProperty : LockableProperty, ITouchableProperty
        {
            protected override void InternalSetLocked(bool lockState)
            {
                throw new NotImplementedException();
            }

            public event EventHandler<EventArgs> Touched;
            public event EventHandler<EventArgs> Untouched;
            
            public bool IsBeingTouched { get; }
            
            public void FastForwardTouch()
            {
                throw new NotImplementedException();
            }
        }
        
        [UnityTest]
        public IEnumerator BuildingSnapZonePutTest()
        {
            // Given a snap zone and snappable property and a builder for a training with a PutIntoSnapZone default step
            GameObject snapZoneGo = new GameObject("SnapZone");
            TrainingSceneObject snapZone = snapZoneGo.AddComponent<TrainingSceneObject>();
            snapZoneGo.AddComponent<DummySnapZoneProperty>();
            snapZone.ChangeUniqueName("SnapZone");

            GameObject putGo = new GameObject("Puttable");
            TrainingSceneObject objectToPut = putGo.AddComponent<TrainingSceneObject>();
            putGo.AddComponent<DummySnappableProperty>();
            objectToPut.ChangeUniqueName("ToPut");

            LinearTrainingBuilder builder = new LinearTrainingBuilder("TestTraining")
                .AddChapter(new LinearChapterBuilder("TestChapter")
                    .AddStep(InteractionDefaultSteps.PutIntoSnapZone("TestSnapZonePutStep", "SnapZone", "ToPut")));

            // When you build a training with it
            IStep step = builder.Build().Data.FirstChapter.Data.FirstStep;

            // Then it has a step with a SnappedCondition
            Assert.True(step != null);
            Assert.True(step.Data.Name == "TestSnapZonePutStep");
            Assert.True(step.Data.Transitions.Data.Transitions.First().Data.Conditions.Count == 1);
            Assert.True(step.Data.Transitions.Data.Transitions.First().Data.Conditions.First() is SnappedCondition);
            Assert.True(ReferenceEquals((step.Data.Transitions.Data.Transitions.First().Data.Conditions.First() as SnappedCondition).Data.Target.Value.SceneObject, objectToPut));

            // Cleanup
            Object.DestroyImmediate(snapZoneGo);
            Object.DestroyImmediate(putGo);

            return null;
        }
        
        [UnityTest]
        public IEnumerator BuildingUseTest()
        {
            // Given a usable property and a builder for a training with Use default step
            GameObject usableGo = new GameObject("Usable");
            TrainingSceneObject usable = usableGo.AddComponent<TrainingSceneObject>();
            usableGo.AddComponent<DummyUsableProperty>();
            usable.ChangeUniqueName("Usable");

            LinearTrainingBuilder builder = new LinearTrainingBuilder("TestTraining")
                .AddChapter(new LinearChapterBuilder("TestChapter")
                    .AddStep(InteractionDefaultSteps.Use("TestUseStep", "Usable")));

            // When you build a training with it
            IStep step = builder.Build().Data.FirstChapter.Data.FirstStep;

            // Then it has a step with an UsedCondition
            Assert.True(step != null);
            Assert.True(step.Data.Name == "TestUseStep");
            Assert.True(step.Data.Transitions.Data.Transitions.First().Data.Conditions.Count == 1);
            Assert.True(step.Data.Transitions.Data.Transitions.First().Data.Conditions.First() is UsedCondition);
            Assert.True(ReferenceEquals((step.Data.Transitions.Data.Transitions.First().Data.Conditions.First() as UsedCondition).Data.UsableProperty.Value.SceneObject, usable));

            // Cleanup
            Object.DestroyImmediate(usableGo);

            return null;
        }
        
        [UnityTest]
        public IEnumerator BuildingTouchTest()
        {
            // Given you have a touchable property and a builder for a training with Touch default step
            GameObject touchableGo = new GameObject("Touchable");
            TrainingSceneObject touchable = touchableGo.AddComponent<TrainingSceneObject>();
            touchableGo.AddComponent<DummyTouchableProperty>();
            touchable.ChangeUniqueName("Touchable");

            LinearTrainingBuilder builder = new LinearTrainingBuilder("TestTraining")
                .AddChapter(new LinearChapterBuilder("TestChapter")
                    .AddStep(InteractionDefaultSteps.Touch("TestTouchStep", "Touchable")));

            // When you build a training with it
            IStep step = builder.Build().Data.FirstChapter.Data.FirstStep;

            // Then it has a step with a TouchCOndition
            Assert.True(step != null);
            Assert.True(step.Data.Name == "TestTouchStep");
            Assert.True(step.Data.Transitions.Data.Transitions.First().Data.Conditions.Count == 1);
            Assert.True(step.Data.Transitions.Data.Transitions.First().Data.Conditions.First() is TouchedCondition);
            Assert.True(ReferenceEquals((step.Data.Transitions.Data.Transitions.First().Data.Conditions.First() as TouchedCondition).Data.TouchableProperty.Value.SceneObject, touchable));

            // Cleanup
            Object.DestroyImmediate(touchableGo);

            return null;
        }
    }
}