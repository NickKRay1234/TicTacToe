using System;
using MVP.Model;
using MVP.TicTacToePresenter;
using UnityEngine;

public class MinMaxAI : MonoBehaviour, IAIStrategy
{
    private const int MAX_SCORE = 10;
    private const int MIN_SCORE = -10;

    // Основной метод для получения лучшего хода
    public CellModel GetBestMove(CellModel[,] gridModels, PlayerMark currentPlayer)
    {
        int bestValue = MIN_SCORE;
        CellModel bestMove = null;

        for (int i = 0; i < gridModels.GetLength(0); i++)
        {
            for (int j = 0; j < gridModels.GetLength(1); j++)
            {
                // Проверяем, свободна ли ячейка
                if (gridModels[i, j].OccupyingPlayer == PlayerMark.None)
                {
                    // Делаем ход и рассчитываем его значение с помощью MinMax
                    gridModels[i, j].OccupyingPlayer = currentPlayer;
                    int moveValue = MinMax(gridModels, 0, false, currentPlayer);

                    // Отменяем ход
                    gridModels[i, j].OccupyingPlayer = PlayerMark.None;

                    // Если значение хода выше текущего лучшего, обновляем лучший ход
                    if (moveValue > bestValue)
                    {
                        bestMove = gridModels[i, j];
                        bestValue = moveValue;
                    }
                }
            }
        }
        return bestMove;
    }

    // Рекурсивная функция MinMax
    private int MinMax(CellModel[,] gridModels, int depth, bool isMaximizing, PlayerMark player)
    {
        int score = Evaluate(gridModels, player);

        // Если игра закончена (выигрыш или проигрыш), возвращаем оценку
        if (score == MAX_SCORE || score == MIN_SCORE)
            return score;

        // Если больше нет ходов и ничья
        if (!IsMovesLeft(gridModels))
            return 0;

        if (isMaximizing)
        {
            int best = MIN_SCORE;

            for (int i = 0; i < gridModels.GetLength(0); i++)
            {
                for (int j = 0; j < gridModels.GetLength(1); j++)
                {
                    if (gridModels[i, j].OccupyingPlayer == PlayerMark.None)
                    {
                        gridModels[i, j].OccupyingPlayer = player;
                        best = Math.Max(best, MinMax(gridModels, depth + 1, !isMaximizing, GetOpponent(player)));
                        gridModels[i, j].OccupyingPlayer = PlayerMark.None;
                    }
                }
            }
            return best;
        }
        else
        {
            int best = MAX_SCORE;

            for (int i = 0; i < gridModels.GetLength(0); i++)
            {
                for (int j = 0; j < gridModels.GetLength(1); j++)
                {
                    if (gridModels[i, j].OccupyingPlayer == PlayerMark.None)
                    {
                        gridModels[i, j].OccupyingPlayer = player;
                        best = Math.Min(best, MinMax(gridModels, depth + 1, !isMaximizing, GetOpponent(player)));
                        gridModels[i, j].OccupyingPlayer = PlayerMark.None;
                    }
                }
            }
            return best;
        }
    }
    
    private bool CheckWin(CellModel[,] gridModels, PlayerMark player)
    {
        // Проходим по всем ячейкам и делаем временные изменения для проверки выигрыша
        for (int i = 0; i < gridModels.GetLength(0); i++)
        {
            for (int j = 0; j < gridModels.GetLength(1); j++)
            {
                if (gridModels[i, j].OccupyingPlayer == PlayerMark.None)
                {
                    // Делаем временный ход
                    gridModels[i, j].OccupyingPlayer = player;

                    // Проверяем, является ли этот ход выигрышным
                    bool isWinningMove = CheckWinEvent?.Invoke(player) ?? false;

                    // Отменяем временный ход
                    gridModels[i, j].OccupyingPlayer = PlayerMark.None;

                    if (isWinningMove)
                    {
                        // Если найден выигрышный ход, возвращаем true
                        return true;
                    }
                }
            }
        }

        // Если выигрышный ход не найден, возвращаем false
        return false;
    }

    // Проверка, остались ли ходы
    private bool IsMovesLeft(CellModel[,] gridModels)
    {
        for (int i = 0; i < gridModels.GetLength(0); i++)
            for (int j = 0; j < gridModels.GetLength(1); j++)
                if (gridModels[i, j].OccupyingPlayer == PlayerMark.None)
                    return true;

        return false;
    }

    // Оценка текущего состояния доски
    private int Evaluate(CellModel[,] gridModels, PlayerMark player)
    {
        // Проверяем, есть ли выигрышная комбинация для игрока
        if (CheckWin(gridModels, player))
            return 10; // Возвращает положительное значение, если игрок выиграл

        // Проверяем, есть ли выигрышная комбинация для противника
        PlayerMark opponent = player == PlayerMark.X ? PlayerMark.O : PlayerMark.X;
        if (CheckWin(gridModels, opponent))
            return -10; // Возвращает отрицательное значение, если противник выиграл

        return 0; // Возвращает 0, если никто не выиграл
    }

    // Получение противоположного игрока
    private PlayerMark GetOpponent(PlayerMark currentPlayer) =>
        currentPlayer == PlayerMark.X ? PlayerMark.O : PlayerMark.X;

    // Метод GetAvailableBestMove необходимо реализовать в соответствии с вашей логикой игры
    public CellModel GetAvailableBestMove(GridPresenter gridPresenter, PlayerMark currentPlayerMark = PlayerMark.O)
    {
        // ...
        return GetBestMove(gridPresenter.Model.GridCells, currentPlayerMark);
    }

    public event Predicate<PlayerMark> CheckWinEvent;
}