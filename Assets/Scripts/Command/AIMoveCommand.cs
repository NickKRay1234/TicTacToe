﻿using MVP.Model;
using SignFactory;
using UnityEngine;
using UnityEngine.UI;

public sealed class AIMoveCommand : BaseCommand
{
    private readonly HeuristicAI _heuristicAI;

    public AIMoveCommand(DesignDataContainer designDataContainer, X_Factory xFactory, O_Factory oFactory, CellPresenter cellPresenter, Transform parent, Image image, CellModel cell, HeuristicAI heuristicAI)
        : base(designDataContainer, xFactory, oFactory, cellPresenter, parent, image, cell)
    {
        _heuristicAI = heuristicAI;
    }

    public override void Execute()
    {
        CellModel bestMove = _heuristicAI.GetAvailableBestMove();
        if (bestMove == null) return;

        Transform parent = bestMove.CellObject.transform;
        Image cellBackground = bestMove.CellObject.GetComponent<Image>();
        PlaceMark(bestMove, PlayerMark.O, parent); // Place AI's mark (typically 'O') on the board.
        _lastMoveTransform = parent;
        _lastMoveImage = cellBackground;
        _lastMoveCell = bestMove;
    }

    protected override PlayerMark GetPlayerMark() => PlayerMark.O;
}