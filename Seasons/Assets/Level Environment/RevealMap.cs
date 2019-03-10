using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class RevealMap : MonoBehaviour
{
    public static RevealMap revealed = null;

    public string g_Tiles = "GrassTilemap";
    public string _Tiles = "Tilemap";

    Tilemap new_map;

    public Grid getBase(bool val)
    {
        Grid to_return = null;
        if (!to_return)
        {
            GameObject grid_obj = GameObject.Find(g_Tiles);
            if(grid_obj && grid_obj.GetComponent<Grid>())
            {
                to_return = grid_obj.GetComponent<Grid>();
            }else if (val)
            {
                grid_obj = new GameObject(g_Tiles);
                to_return = grid_obj.AddComponent<Grid>();
            }

        }
        return to_return;
    }

    public Tilemap getTileMap(string name)
    {
        GameObject temp = GameObject.Find(name);
        return temp != null ? temp.GetComponent<Tilemap>() : null;
    }

    public void DeleteTile(Vector3Int position, Tilemap tilemap)
    {
        tilemap.SetTile(position, null);
    }

    public void RevealTiles(Vector3 objectCoord, TileBase tiles, Transform tile)
    {
        Grid temp_grid = getBase(false);
        GridLayout layout_grid = temp_grid.GetComponent<GridLayout>();
        Vector3Int coord_grid = layout_grid.LocalToCell(objectCoord);

        Tilemap t_map = getTileMap(g_Tiles);
        new_map = getTileMap(_Tiles);
        new_map.SetTile(coord_grid, tiles);
        DeleteTile(coord_grid, t_map);

    }
}
