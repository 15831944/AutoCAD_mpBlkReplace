﻿namespace mpBlkReplace
{
    using System.Collections.Generic;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.EditorInput;
    using Autodesk.AutoCAD.Geometry;
    using Autodesk.AutoCAD.Runtime;
    using ModPlusAPI;
    using ModPlusAPI.Windows;
    using AcApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

    public class MpBlkReplace
    {
        private static string _langItem = new ModPlusConnector().Name;

        private static bool _scales;
        private static bool _transform;
        private static bool _layer;
        private static bool _rotation;
        private static int _cleanBd;

        private static void ReplaceAll()
        {
            var doc = AcApp.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            var blkNamesToRemove = new List<string>();
            try
            {
                var peo = new PromptEntityOptions("\n" + Language.GetItem(_langItem, "h1") + ": ");
                peo.SetRejectMessage("\n" + Language.GetItem(_langItem, "wrong"));
                peo.AllowNone = false;
                peo.AllowObjectOnLockedLayer = true;
                peo.AddAllowedClass(typeof(BlockReference), true);
                var per = ed.GetEntity(peo);
                if (per.Status != PromptStatus.OK)
                    return;
                var secondBlockId = per.ObjectId;

                peo.Message = "\n" + Language.GetItem(_langItem, "h2") + ": ";
                per = ed.GetEntity(peo);
                if (per.Status != PromptStatus.OK)
                    return;
                var firstBlockId = per.ObjectId;

                var acTypValAr = new TypedValue[1];
                acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
                var acSelFtr = new SelectionFilter(acTypValAr);
                var selection = ed.SelectAll(acSelFtr);
                if (selection.Status != PromptStatus.OK)
                    return;
                var acSSet = selection.Value;
                using (var tr = doc.TransactionManager.StartTransaction())
                {
                    var firstBlkPosition = (tr.GetObject(firstBlockId, OpenMode.ForRead) as BlockReference)?.Position;
                    var secondBlkName = (tr.GetObject(secondBlockId, OpenMode.ForRead) as BlockReference)?.Name;
                    foreach (SelectedObject selObj in acSSet)
                    {
                        var blockInSelection = tr.GetObject(selObj.ObjectId, OpenMode.ForRead) as BlockReference;
                        if (blockInSelection != null && blockInSelection.Name.Equals(secondBlkName))
                        {
                            var layerId = blockInSelection.LayerId;
                            var scales = blockInSelection.ScaleFactors;
                            var transform = blockInSelection.BlockTransform;
                            var rotation = blockInSelection.Rotation;

                            if (!blkNamesToRemove.Contains(blockInSelection.Name))
                                blkNamesToRemove.Add(blockInSelection.Name);
                            blockInSelection.UpgradeOpen();
                            blockInSelection.Erase(true);

                            var collection = new ObjectIdCollection
                            {
                                firstBlockId
                            };
                            var mapping = new IdMapping();
                            db.DeepCloneObjects(collection, db.CurrentSpaceId, mapping, false);

                            var idPair = mapping[firstBlockId];
                            var blk = tr.GetObject(idPair.Value, OpenMode.ForWrite) as BlockReference;
                            var vector3D = blockInSelection.Position - firstBlkPosition;
                            if (vector3D != null)
                            {
                                var movementMat = Matrix3d.Displacement((Vector3d)vector3D);
                                blk?.TransformBy(movementMat);

                                // transform
                                TransformBlock(blk, transform, scales, layerId, rotation);
                            }
                        }
                    }

                    tr.Commit();
                }

                if (blkNamesToRemove.Count > 0)
                    RemoveBlocksFromDataBase(blkNamesToRemove);
            }
            catch (System.Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }

        private static void ReplaceSelected(PromptSelectionResult psr)
        {
            var doc = AcApp.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            var blkNamesToRemove = new List<string>();
            try
            {
                // Если начальный выбор отсутствовал
                var selSet = psr.Value;
                if (selSet == null || selSet.Count == 0)
                {
                    var pso = new PromptSelectionOptions
                    {
                        AllowDuplicates = false,
                        AllowSubSelections = false,
                        MessageForAdding = "\n" + Language.GetItem(_langItem, "h3") + ": ",
                        MessageForRemoval = "\n" + Language.GetItem(_langItem, "h4") + ": "
                    };

                    var acTypValAr = new TypedValue[1];
                    acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
                    var acSelFtr = new SelectionFilter(acTypValAr);
                    var psrNew = ed.GetSelection(pso, acSelFtr);
                    if (psrNew.Status != PromptStatus.OK)
                        return;
                    selSet = psrNew.Value;
                }

                // Снова проверяем на количество
                if (selSet.Count > 0)
                {
                    var peo = new PromptEntityOptions(string.Empty);
                    peo.SetRejectMessage("\n" + Language.GetItem(_langItem, "wrong"));
                    peo.AllowNone = false;
                    peo.AllowObjectOnLockedLayer = true;
                    peo.AddAllowedClass(typeof(BlockReference), true);
                    peo.Message = "\n" + Language.GetItem(_langItem, "h2") + ": ";
                    var per = ed.GetEntity(peo);
                    if (per.Status != PromptStatus.OK)
                        return;
                    var firstBlockId = per.ObjectId;
                    using (var tr = doc.TransactionManager.StartTransaction())
                    {
                        var firstBlkPosition = (tr.GetObject(firstBlockId, OpenMode.ForRead) as BlockReference)?.Position;
                        var firstBlkName = (tr.GetObject(firstBlockId, OpenMode.ForRead) as BlockReference)?.Name;

                        foreach (SelectedObject selObj in selSet)
                        {
                            var blockInSelection = tr.GetObject(selObj.ObjectId, OpenMode.ForRead) as BlockReference;
                            if (blockInSelection != null && !blockInSelection.Name.Equals(firstBlkName))
                            {
                                var layerId = blockInSelection.LayerId;
                                var scales = blockInSelection.ScaleFactors;
                                var transform = blockInSelection.BlockTransform;
                                var rotation = blockInSelection.Rotation;

                                if (!blkNamesToRemove.Contains(blockInSelection.Name))
                                    blkNamesToRemove.Add(blockInSelection.Name);
                                blockInSelection.UpgradeOpen();
                                blockInSelection.Erase(true);

                                var collection = new ObjectIdCollection { firstBlockId };
                                var mapping = new IdMapping();
                                db.DeepCloneObjects(collection, db.CurrentSpaceId, mapping, false);

                                var idPair = mapping[firstBlockId];
                                var blk = tr.GetObject(idPair.Value, OpenMode.ForWrite) as BlockReference;
                                var vector3D = blockInSelection.Position - firstBlkPosition;
                                if (vector3D != null)
                                {
                                    var movementMat = Matrix3d.Displacement((Vector3d)vector3D);
                                    blk?.TransformBy(movementMat);

                                    // transform
                                    TransformBlock(blk, transform, scales, layerId, rotation);
                                }
                            }
                        }

                        tr.Commit();
                    }

                    if (blkNamesToRemove.Count > 0)
                        RemoveBlocksFromDataBase(blkNamesToRemove);
                }
            }
            catch (System.Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }

        private static void RemoveBlocksFromDataBase(List<string> blkNames)
        {
            if (_cleanBd.Equals(2))
                return;
            var doc = AcApp.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            try
            {
                if (_cleanBd.Equals(0))
                {
                    var pko = new PromptKeywordOptions(string.Empty)
                    {
                        Message = "\n" + Language.GetItem(_langItem, "h5") + ": ",
                        AllowNone = false,
                        AppendKeywordsToMessage = true
                    };
                    pko.Keywords.Add("Yes", Language.GetItem(_langItem, "yes"));
                    pko.Keywords.Add("No", Language.GetItem(_langItem, "no"));
                    var pkr = ed.GetKeywords(pko);
                    if (pkr.Status != PromptStatus.OK)
                        return;
                    if (pkr.StringResult.Equals("Yes"))
                    {
                        RemoveBlocks(blkNames, doc, db);
                    }
                }

                if (_cleanBd.Equals(1))
                    RemoveBlocks(blkNames, doc, db);
            }
            catch (System.Exception exception)
            {
                ExceptionBox.Show(exception);
            }
        }

        private static void RemoveBlocks(IEnumerable<string> blkNames, Document doc, Database db)
        {
            using (var tr = doc.TransactionManager.StartTransaction())
            {
                var bt = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;

                foreach (var blkName in blkNames)
                {
                    if (bt != null && bt.Has(blkName))
                    {
                        var btr = tr.GetObject(bt[blkName], OpenMode.ForWrite) as BlockTableRecord;
                        if (btr != null && (btr.GetBlockReferenceIds(false, false).Count == 0 && !btr.IsLayout))
                            btr.Erase(true);
                    }
                }

                tr.Commit();
            }
        }

        private static void TransformBlock(BlockReference blk, Matrix3d transform, Scale3d scales, ObjectId layerId, double rotation)
        {
            if (_transform)
                blk.BlockTransform = transform;
            if (_scales)
                blk.ScaleFactors = scales;
            if (_layer)
                blk.LayerId = layerId;
            if (_rotation)
                blk.Rotation = rotation;
        }

        private static void GetSettings()
        {
            _layer = bool.TryParse(UserConfigFile.GetValue(_langItem, "layer"), out bool b) && b;
            _transform = bool.TryParse(UserConfigFile.GetValue(_langItem, "transform"), out b) && b;
            _scales = bool.TryParse(UserConfigFile.GetValue(_langItem, "scales"), out b) && b;
            _rotation = bool.TryParse(UserConfigFile.GetValue(_langItem, "rotation"), out b) && b;
            _cleanBd = int.TryParse(UserConfigFile.GetValue(_langItem, "cleanBD"), out int i) ? i : 0;
        }

        [CommandMethod("ModPlus", "mpBlkReplace", CommandFlags.UsePickSet)]
        public static void Main()
        {
            Statistic.SendCommandStarting(new ModPlusConnector());
            GetSettings();
            var repeat = true;
            var doc = AcApp.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            while (repeat)
            {
                var preSelection = ed.SelectImplied();

                var pko = new PromptKeywordOptions(string.Empty)
                {
                    Message = "\n" + Language.GetItem(_langItem, "h6") + ": ",
                    AllowNone = false,
                    AppendKeywordsToMessage = true
                };
                pko.Keywords.Add("replaceSelected", Language.GetItem(_langItem, "k1"));
                pko.Keywords.Add("replaceAll", Language.GetItem(_langItem, "k2"));
                pko.Keywords.Add("seTtings", Language.GetItem(_langItem, "k3"));

                var pkr = ed.GetKeywords(pko);
                if (pkr.Status != PromptStatus.OK)
                    return;

                switch (pkr.StringResult)
                {
                    case "replaceSelected":
                        repeat = false;
                        ReplaceSelected(preSelection);
                        break;
                    case "replaceAll":
                        repeat = false;
                        ReplaceAll();
                        break;
                    case "seTtings":
                        var win = new Settings();
                        win.ShowDialog();
                        GetSettings();
                        break;
                }
            }
        }
    }
}
