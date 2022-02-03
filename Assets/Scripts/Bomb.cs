using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private LayerMask explossionMask;
    [SerializeField] private GameObject explosion;
    private ContactFilter2D _contactFilter2D = new ContactFilter2D();
    private List<RaycastHit2D> _raycastHit2Ds = new List<RaycastHit2D>();
    
    private List<Bomb> _bombs;
    private List<Vector3> exploresPos = new List<Vector3>();
    private List<GameObject> toDestroy = new List<GameObject>();

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);

        Raycast(Vector2.up);
        Raycast(Vector2.down);
        Raycast(Vector2.left);
        Raycast(Vector2.right);

        InstantiateExplossions();
        DestroyObjects();

        yield return null;
        
        Destroy(gameObject);
    }

    private void DestroyObjects()
    {
        for (int i = toDestroy.Count - 1; i > 0; i--)
        {
            Destroy(toDestroy[i]);
        }
    }

    private void InstantiateExplossions()
    {
        exploresPos.Add(transform.position);
        foreach (var explore in exploresPos)
        {
            Instantiate(explosion, explore, Quaternion.identity);
        }
    }

    private void Raycast(Vector2 dir)
    {
        int results = Physics2D.Raycast(transform.position, dir,
            _contactFilter2D, _raycastHit2Ds, 2f);

        for (int i = 0; i < 2; i++)
        {
            exploresPos.Add((Vector2)transform.position + (dir * (i + 1)));
        }

        for (int i = 0; i < results; i++)
        {
            var hitLayer = _raycastHit2Ds[i].transform.gameObject.layer;

            if (hitLayer == 3)
            {
                if ((int)dir.y == 0)
                {
                    var countToRemove =
                        Mathf.Abs((int) (_raycastHit2Ds[i].transform.position.x - transform.position.x));
                    RemovePositionsForExplossion(countToRemove);
                    break;
                }

                if ((int)dir.x == 0)
                {
                    var countToRemove =
                        Mathf.Abs((int) (_raycastHit2Ds[i].transform.position.y - transform.position.y));
                    RemovePositionsForExplossion(countToRemove);
                    break;
                }
            }

            if (hitLayer == 6 || hitLayer == 7 || hitLayer == 8 || hitLayer == 9)
            {
                toDestroy.Add(_raycastHit2Ds[i].transform.gameObject);
            }
        }
    }

    private void RemovePositionsForExplossion(int dir)
    {
        var count = dir - 1;
        
        for (int i = 0; i < 2 - count; i++)
        {
            exploresPos.RemoveAt(exploresPos.Count - 1);
        }
    }
}
