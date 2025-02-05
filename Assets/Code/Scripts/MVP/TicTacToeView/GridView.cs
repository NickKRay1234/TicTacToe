﻿using System;
using MVP.Model;
using MVP.TicTacToePresenter;
using SignFactory;
using UnityEngine;
using VContainer;

namespace MVP.TicTacToeView
{
    [HelpURL("https://unity.com/how-to/build-modular-codebase-mvc-and-mvp-programming-patterns")]
    public sealed class GridView : MonoBehaviour, IGridCleanable
    {
        [Inject] private DesignDataContainer _designDataContainer;
        [Inject] private Cell_Factory _cellFactory;
        [Inject] private X_Factory _xFactory;
        [Inject] private O_Factory _oFactory;
        private GridPresenter _gridPresenter;

        public GridPresenter Presenter
        {
            get => _gridPresenter;
            private set => _gridPresenter = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// Initializes the grid with cells
        public void InitializeGrid()
        {
            Presenter ??= new GridPresenter(new GridModel(_designDataContainer.GRID_SIZE), this);
            _designDataContainer.CurrentPlayer = _designDataContainer.InitialPlayer;
            CreateGridCells();
        }

        /// Populates the grid with cells
        private void CreateGridCells()
        {
            for (int rowIndex = 0; rowIndex < _designDataContainer.GRID_SIZE; rowIndex++)
            for (int colIndex = 0; colIndex < _designDataContainer.GRID_SIZE; colIndex++)
                InitializeGridCell(rowIndex, colIndex);
        }

        /// Initializes an individual cell
        private void InitializeGridCell(int rowIndex, int colIndex)
        {
            CellModel cellModel = new CellModel(rowIndex, colIndex);
            Presenter.Model.GridCells[rowIndex, colIndex] = cellModel;
            CreateCellGameObject(cellModel);
        }

        /// Creates the GameObject for a cell
        private void CreateCellGameObject(CellModel cellModel)
        {
            IProduct product = _cellFactory.GetProduct(transform);
            GameObject cellBody = product.GetGameObject();
            CellView cellView = cellBody.GetComponent<CellView>();
            cellModel.CellObject = cellBody;
            CommandFactory commandFactory = new CommandFactory(_gridPresenter, _designDataContainer);
            //new CellPresenter(cellModel, cellView, commandFactory, _designDataContainer);
            cellView.Presenter = new CellPresenter(cellModel, cellView, commandFactory, _designDataContainer);
        }

        /// Destroys a single cell object
        private void DestroyCell(int row, int col)
        {
            GameObject cellBody = Presenter.Model.GridCells[row, col].CellObject;
            if (cellBody)
            {
                Destroy(cellBody);
                Presenter.Model.GridCells[row, col].CellObject = null;
            }
        }

        /// Destroying each cell game object in the grid.
        public void ClearGrid()
        {
            for (int rowIndex = 0; rowIndex < _designDataContainer.GRID_SIZE; rowIndex++)
            for (int colIndex = 0; colIndex < _designDataContainer.GRID_SIZE; colIndex++)
                DestroyCell(rowIndex, colIndex);
        }
    }
}