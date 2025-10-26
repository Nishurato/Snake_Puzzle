using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Snake
{
    public Transform head;
    public List<Transform> segments = new List<Transform>();
    public Transform bodyPrefab;
    public Vector3 spawnDirection = Vector3.left; // Направление спавна сегментов

    private Tilemap groundMap;
    private Tilemap wallMap;

    public Vector2 LastMoveDirection { get; private set; } = Vector2.right;
    public bool IsMoving { get; private set; }

    public void Init(Tilemap ground, Tilemap wall)
    {
        groundMap = ground;
        wallMap = wall;

        segments.Clear();
        segments.Add(head);

        // Добавляем 2 сегмента тела по умолчанию
        for (int i = 1; i <= 2; i++)
        {
            Transform segment = Object.Instantiate(bodyPrefab);
            segment.position = head.position + spawnDirection * i;
            segments.Add(segment);
        }
    }
    public void SetLastDirection(Vector2 dir)
    {
        LastMoveDirection = dir;
    }
    public bool IsOppositeDirection(Vector2 dir)
    {
        if (segments.Count > 1)
        {
            Vector3Int targetCell = groundMap.WorldToCell(head.position + (Vector3)dir);
            Vector3Int firstBodyCell = groundMap.WorldToCell(segments[1].position);
            return targetCell == firstBodyCell;
        }
        return false;
    }


    public bool CanMove(Vector2 dir)
    {
        Vector3Int targetCell = groundMap.WorldToCell(head.position + (Vector3)dir);

        // Проверка на тайлы земли и стены
        if (!groundMap.HasTile(targetCell) || wallMap.HasTile(targetCell))
            return false;

        // Проверка на собственное тело
        foreach (var seg in segments)
        {
            if (groundMap.WorldToCell(seg.position) == targetCell)
                return false;
        }

        return true;
    }

    public IEnumerator MoveStep(Vector2 dir, float moveTime)
    {
        if (dir == Vector2.zero)
        {
            yield break; // не двигаем змейку, если направление нулевое
        }

        IsMoving = true;
        SetLastDirection(dir);

        // Сохраняем предыдущие позиции
        List<Vector3> previousPositions = new List<Vector3>();
        foreach (var seg in segments)
            previousPositions.Add(seg.position);

        int prevCount = previousPositions.Count;

        Vector3 startPos = head.position;
        Vector3 targetPos = startPos + (Vector3)dir;
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveTime);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            head.position = Vector3.Lerp(startPos, targetPos, smoothT);

            for (int i = 1; i < segments.Count; i++)
            {
                if (i < prevCount)
                {
                    segments[i].position = Vector3.Lerp(previousPositions[i], previousPositions[i - 1], smoothT);
                }
                else
                {
                    // новый сегмент: поместим его в позицию последнего известного хвоста
                    segments[i].position = previousPositions[prevCount - 1];
                }
            }

            yield return null;
        }

        // Финальная установка позиций
        head.position = targetPos;
        for (int i = 1; i < segments.Count; i++)
        {
            if (i - 1 < prevCount)
                segments[i].position = previousPositions[i - 1];
            else
                segments[i].position = previousPositions[prevCount - 1];
        }

        IsMoving = false;
    }

    public bool CanMoveInLastDirection()
    {
        return CanMove(LastMoveDirection);
    }

    public void Grow()
    {
        Transform tail = segments[segments.Count - 1]; // Последний сегмент
        Transform newSegment = Object.Instantiate(bodyPrefab, tail.position, Quaternion.identity);
        segments.Add(newSegment);
    }
}