﻿using System;
using System.Collections.Generic;

namespace UpdateFieldCodeGenerator.Formats
{
    public class UpdateFieldHandlers : IDisposable
    {
        private readonly ICollection<IUpdateFieldHandler> _handlers;
        private readonly Dictionary<IUpdateFieldHandler, IReadOnlyList<FlowControlBlock>> _previousControlFlowDict;

        public UpdateFieldHandlers()
        {
            _handlers = new List<IUpdateFieldHandler>()
            {
                new TrinityCoreHandler(),
                new WowPacketParserHandler()
            };
            _previousControlFlowDict = new Dictionary<IUpdateFieldHandler, IReadOnlyList<FlowControlBlock>>();
            foreach (var handler in _handlers)
                _previousControlFlowDict[handler] = null;
        }

        public void BeforeStructures()
        {
            foreach (var handler in _handlers)
                handler.BeforeStructures();
        }

        public void AfterStructures()
        {
            foreach (var handler in _handlers)
                handler.AfterStructures();
        }

        public void OnStructureBegin(Type structureType, ObjectType objectType, bool create, bool writeUpdateMasks)
        {
            foreach (var handler in _handlers)
                handler.OnStructureBegin(structureType, objectType, create, writeUpdateMasks);
        }

        public void OnStructureEnd(bool needsFlush, bool hadArrayFields)
        {
            FinishControlBlocks();
            foreach (var handler in _handlers)
                handler.OnStructureEnd(needsFlush, hadArrayFields);
        }

        public void OnField(string name, UpdateField updateField)
        {
            foreach (var handler in _handlers)
                _previousControlFlowDict[handler] = handler.OnField(name, updateField, _previousControlFlowDict[handler]);
        }

        public void OnDynamicFieldSizeCreate(string name, UpdateField updateField)
        {
            foreach (var handler in _handlers)
                _previousControlFlowDict[handler] = handler.OnDynamicFieldSizeCreate(name, updateField, _previousControlFlowDict[handler]);
        }

        public void OnDynamicFieldSizeUpdate(string name, UpdateField updateField)
        {
            foreach (var handler in _handlers)
                _previousControlFlowDict[handler] = handler.OnDynamicFieldSizeUpdate(name, updateField, _previousControlFlowDict[handler]);
        }

        public void FinishControlBlocks()
        {
            foreach (var handler in _handlers)
            {
                handler.FinishControlBlocks(_previousControlFlowDict[handler]);
                _previousControlFlowDict[handler] = null;
            }
        }

        public void FinishBitPack()
        {
            foreach (var handler in _handlers)
                handler.FinishBitPack();
        }

        public void Dispose()
        {
            foreach (var handler in _handlers)
                handler.Dispose();

            _handlers.Clear();
        }
    }
}