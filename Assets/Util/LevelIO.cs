using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static ChunkLoader;

public class LevelIO {
    public static Block[] palette; //starts from 1
    private static Dictionary<Block, int> unpalette = new Dictionary<Block, int>(); //starts from 0

    public static Level read(string path) {
        Level level = null;
        if(File.Exists(path)) {
            using(FileStream stream = File.Open(path, FileMode.Open)) {
                using(BinaryReader read = new BinaryReader(stream, Encoding.UTF8)) {
                    level = readMeta(read.ReadString());
                    Debug.Log("Successfully read meta!");
                    level.blocks = new List<BlockSave>();

                    int size = read.ReadInt32();
                    Debug.Log("Read size: " + size);
                    int maxID = palette.Length - 1;

                    int chunk = 0;
                    for(int i = 0; i < size; i++) {
                        ushort id = read.ReadUInt16(); //id starts from 1
                        if(id == 0) {
                            chunk++;
                            //Debug.Log("[New Chunk]");
                            continue;
                        }
                        Block b = id > maxID ? palette[0] : palette[id]; //palette starts from 1
                        byte ctype = read.ReadByte();
                        int bx = read.ReadByte() + chunk * 256;
                        int by = read.ReadByte();

                        level.blocks.Add(new BlockSave(b, bx / 2f, by / 2f, ctype));
                    }
                    Debug.Log("Successfully read level!");
                }
            }
        }

        return level;
    }

    public static bool write(Level level, string path) {

        using(FileStream stream = File.Open(path, FileMode.Create)) {
            using(BinaryWriter write = new BinaryWriter(stream, Encoding.UTF8)) {
                //write palette
                level.palette = new string[Vars.main.content.blocks.Length];
                unpalette.Clear();
                for(int i = 0; i < Vars.main.content.blocks.Length; i++) {
                    level.palette[i] = Vars.main.content.blocks[i].name;
                    unpalette[Vars.main.content.blocks[i]] = i;
                }
                level.themeName = level.theme == null ? "Plains" : level.theme.name;

                //write level
                string levelJson = JsonUtility.ToJson(level);
                write.Write(levelJson);
                Debug.Log("Encoded L:"+ levelJson);
                int chunks = level.blocks.Count <= 0 ? 0 : pos(level.blocks[level.blocks.Count - 1].x) / 256;
                //Debug.Log("Writing level! Estimated chunks: "+chunks);

                int size = level.blocks.Count + chunks;
                Debug.Log("Estimated size: "+size);
                write.Write(size);

                int chunk = 0;
                foreach(BlockSave bs in level.blocks) {
                    int bx = pos(bs.x);
                    while(bx / 256 > chunk) {
                        //insert new chunk flag
                        write.Write((ushort)0);
                        //Debug.Log("[New Chunk]");
                        chunk++;
                    }

                    bx %= 256;
                    write.Write((ushort) (unpalette[bs.type] + 1));
                    write.Write(bs.ctype);
                    write.Write((byte) bx);
                    write.Write((byte) pos(bs.y));
                }
                Debug.Log("Successfully wrote level!");
            }
        }

        return true;
    }

    //generates a Level class with appropriate fields and sets the palette
    private static Level readMeta(string levelJson) {
        Debug.Log("Decoding L:"+levelJson);
        Level l = JsonUtility.FromJson<Level>(levelJson);
        if(palette == null) palette = new Block[Vars.main.content.blocks.Length + 1]; //block type of 0 is a special flag; leave it empty and count from 1
        Block missing = Vars.main.content.block("Wall"); //todo replace with "Missing"

        l.theme = Vars.main.content.theme(l.themeName == "" ? "Plains" : l.themeName);

        //generate block palette
        for(int i = 0; i < palette.Length - 1; i++) {
            if(i >= l.palette.Length) {
                palette[i + 1] = missing;
                continue;
            }
            Block b = Vars.main.content.block(l.palette[i]);
            if(b == null) b = missing;
            palette[i + 1] = b;
        }
        palette[0] = missing;

        return l;
    }

    //returns a temporary level with only the meta fetched
    public static Level fetchMeta(string path) {
        Level level = null;
        if(File.Exists(path)) {
            using(FileStream stream = File.Open(path, FileMode.Open)) {
                using(BinaryReader read = new BinaryReader(stream, Encoding.UTF8)) {
                    level = JsonUtility.FromJson<Level>(read.ReadString());
                    level.theme = Vars.main.content.theme(level.themeName == "" ? "Plains" : level.themeName);
                }
            }
        }

        return level;
    }

    private static int pos(float wpos) {
        return Mathf.RoundToInt(wpos * 2f);
    }
}
