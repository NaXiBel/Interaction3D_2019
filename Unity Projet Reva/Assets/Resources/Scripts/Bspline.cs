using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class Bspline : MonoBehaviour
{

    //private IntPtr m_Bspline = IntPtr.Zero;
    public float[] xcontr;
    public float[] ycontr;
    public float[] zcontr;

    //nombre de points de controle en u et v
    public int nu, nv;
    //coordonnees des points calcules
    public Vector3[] vcal;
    //nombre de points dalcules par axe u et v
    private int nmaxu, nmaxv;
    //ordre de la courbe en u et v
    public int ku, kv;
    //vecteurs de noeuds en u et v
    private int[] vknotu, vknotv;
    //indice max des vecteurs de noeuds
    private int imaxu, imaxv;

    //points du maillage
    Vector3[] pts_mesh;
    //triangles du maillage
    int[] triangles;
    //uv du maillage
    Vector2[] uvs;

    public void Start()
    {

        //nombre de points en u et v
        nu = nv = 5;

        //initialisation de points de contrôle par defaut
        xcontr = new float[nu * nv];
        ycontr = new float[nu * nv];
        zcontr = new float[nu * nv];
        /*ifstream points("points.pts", ios::in);
		if (points)
		{*/

        using (TextReader reader = File.OpenText("points.pts"))
        {
            string text = reader.ReadLine();
            string[] bits = text.Split(' ');

            for (int i = 0; i < nv; i++)
                for (int j = 0; j < nu; j++)
                    xcontr[j + i * nu] = float.Parse(bits[j + i * nu]);

            text = reader.ReadLine();
            bits = text.Split(' ');
            for (int i = 0; i < nv; i++)
                for (int j = 0; j < nu; j++)
                    ycontr[j + i * nu] = float.Parse(bits[j + i * nu]);

            text = reader.ReadLine();
            bits = text.Split(' ');
            for (int i = 0; i < nv; i++)
                for (int j = 0; j < nu; j++)
                    zcontr[j + i * nu] = float.Parse(bits[j + i * nu]);
        }

        /*for (int i = 0; i < 25; i++)
            Debug.Log(String.Format("{0} {1} {2} \n", xcontr[i], ycontr[i], zcontr[i]));*/

        /*points.close();
    }*/

        //nombre maximum de points calcules par axe u , vecteur
        nmaxu = nmaxv = 20;

        //ordre de la B-Spline en u et v
        ku = kv = 4;

        //points de la surface
        vcal = new Vector3[nmaxu * nmaxv];

        //initialisation des triangles du maillage
        //triangles = new int[2 * (nmaxu - 1) * (nmaxv - 1) * 2 * 3];
        triangles = new int[(nmaxu - 1) * (nmaxv - 1) * 2 * 3];

        int ind = 0;

        for (int i = 0; i < nmaxu * (nmaxv - 1); i++)
        {
            if (i % nmaxu != 0)
            {
                /*triangles[ind] = i;
                triangles[ind + 1] = i + nmaxu - 1;
                triangles[ind + 2] = i + nmaxu;*/
                triangles[ind + 0] = i;
                triangles[ind + 1] = i + nmaxu;
                triangles[ind + 2] = i + nmaxu - 1;

                /*Debug.Log(String.Format("f {0} {1} {2}", i, i + (nmaxu - 1) - 1, i + (nmaxu - 1)));
                Debug.Log(String.Format("f {0} {1} {2}", i, i + (nmaxu - 1), i + (nmaxu - 1) - 1));*/

                ind += 3;
            }
            if (i % nmaxu != (nmaxu - 1))
            {
                /*triangles[ind] = i;
                triangles[ind + 1] = i + nmaxu;
                triangles[ind + 2] = i + 1;*/
                triangles[ind + 0] = i;
                triangles[ind + 1] = i + 1;
                triangles[ind + 2] = i + nmaxu;

                /*Debug.Log(String.Format("f {0} {1} {2}", i, i + (nmaxu - 1), i + 1));
                Debug.Log(String.Format("f {0} {1} {2}", i, i + 1, i + (nmaxu - 1)));*/

                ind += 3;
            }
        }

    }

    //calcule de tout les points de la surface
    public void Calc()
    {

        //calcul des vecteurs de noeuds
        CalcKnots();

        int k = 0;

        //calcul et ponderation des points de la surface
        for (int jv = 1; jv <= nmaxv; jv++)
        {
            float v = (float)((jv - 1) * vknotv[imaxv]) / (nmaxv - 1);
            for (int ju = 1; ju <= nmaxu; ju++)
            {
                float u = (float)((ju - 1) * vknotu[imaxu]) / (nmaxu - 1);
                //cout << "u: " << u << endl;
                Cals3X(u, v, k);
                k++;
            }
        }

        //changement du maillage de l'objet
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        //mesh.Clear();

        //pts_mesh = new Vector3[1444 * 3];
        uvs = new Vector2[nmaxu * nmaxv];

        /*for (int i = 0; i < nmaxu * nmaxv; i++)
            uvs[i] = new Vector2(vcal[i].x, vcal[i].z);*/

        mesh.vertices = vcal;
        mesh.triangles = triangles;
        //mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        //ecriture du maillage de la surface dans Bspline.obj
        //Save();
    }

    //calcul du vecteur de noeud en u et v
    private void CalcKnots()
    {
        int max;

        vknotu = new int[10];
        vknotv = new int[10];

        //calcul du vecteur de noeud en u
        max = (nu - 1) - ku + 2;

        //Debug.Log(String.Format("max: {0}", max));

        for (int i = 0; i <= ku - 2; i++)
            vknotu[i] = 0;
        for (int i = 0; i <= max; i++)
            vknotu[i + ku - 1] = i;
        for (int i = 0; i <= ku - 2; i++)
            vknotu[i + ku + max] = max;

        imaxu = (nu - 1) + ku;

        /*string s = "";
        for (int i = 0; i <= ku - 2 + ku + max; i++)
            s += string.Format("{0} ", vknotu[i]);
            Debug.Log(s);
        /*cout << vknotu[i] << " ";
        cout << endl;*/

        //calcul du vecteur de noeud en v
        max = (nv - 1) - kv + 2;

        for (int i = 0; i <= kv - 2; i++)
            vknotv[i] = 0;
        for (int i = 0; i <= max; i++)
            vknotv[i + kv - 1] = i;
        for (int i = 0; i <= kv - 2; i++)
            vknotv[i + kv + max] = max;

        imaxv = (nv - 1) + kv;
    }

    private void Cals3X(float u, float v, int k)
    {
        int iu, iv;
        float[] bu = new float[20];
        float[] bv = new float[20];

        iv = iu = -1;
        Posit(u, v, ref iu, ref iv);

        //Debug.Log(String.Format("iu: {0} iv :{1}", iu, iv));

        vcal[k] = new Vector3(0, 0, 0);

        if (iu > 0 && iv > 0)
        {
            CalBSp(u, v, bu, bv, iu, iv);

            /*//test des fonctions calculees avec calBSp
            float testu = 0;
            float testv = 0;
            for(int j = 1; j <= kv; j++)
            testu += bu[j];
            for(int j = 1; j<=ku; j++)
            testv += bv[j];
            Debug.Log(String.Format("testu: {0} testv :{1}", testu, testv));
            //cout << testu << " " << testv <<endl;*/

            for (int j = 1; j <= kv; j++)
            {
                int ical1 = (iv - kv + j) * nu + iu - ku;
                for (int i = 1; i <= ku; i++)
                {
                    float xx = bu[i] * bv[j];
                    int ical2 = ical1 + i;
                    vcal[k].x += xx * xcontr[ical2];
                    vcal[k].y += xx * ycontr[ical2];
                    vcal[k].z += xx * zcontr[ical2];
                }
            }
        }

    }

    private void Posit(float u, float v, ref int iu, ref int iv)
    {
        int xmin, xmax;

        //positionnement dans le vecteur de noeud en u
        xmin = vknotu[0];
        xmax = vknotu[imaxu];
        if (u >= xmin && u <= xmax)
        {
            if (u == xmax)
            {
                iu = imaxu - ku;
                while (vknotu[iu] == vknotu[iu + 1] && iu > 0)
                    iu--;
                if (iu == 0)
                    iu = -2;
            }
            else
            {
                iu = ku - 1;
                while (u >= vknotu[iu] && iu < imaxu)
                    iu++;
                if (iu == imaxu)
                    iu = -2;
                else
                    iu--;
            }
        }
        else
            iu = -1;
        //cout << "iu: " << iu << endl; 

        //positionnement dans le vecteur de noeud en v
        xmin = vknotv[0];
        xmax = vknotv[imaxv];
        if (v >= xmin && v <= xmax)
        {
            if (v == xmax)
            {
                iv = imaxv - kv;
                while (vknotv[iv] == vknotv[iv + 1] && iv > 0)
                    iv--;
                if (iv == 0)
                    iv = -2;
            }
            else
            {
                iv = kv - 1;
                while (v >= vknotu[iv] && iv < imaxv)
                    iv++;
                if (iv == imaxv)
                    iv = -2;
                else
                    iv--;
            }
        }
        else
            iv = -1;
    }

    private void CalBSp(float u, float v, float[] bu, float[] bv, int iu, int iv)
    {
        float bb;

        //calcul du point en u
        bu[1] = 1;
        int iu1 = iu + 1;
        bb = bu[1];
        bu[1] = (float)(bb * (vknotu[iu1] - u)) / (vknotu[iu1] - vknotu[iu]);
        bu[2] = (float)(bb * (u - vknotu[iu])) / (vknotu[iu1] - vknotu[iu]);

        for (int l = 3; l <= ku; l++)
        {
            bb = bu[1];
            bu[1] = (float)(bb * (vknotu[iu1] - u)) / (vknotu[iu1] - vknotu[iu - l + 2]);
            for (int m = 2; m < l; m++)
            {
                int im = iu + m;
                int ilm = iu - l + m;
                float bh = bb;
                bb = bu[m];
                bu[m] = (float)(bh * (u - vknotu[ilm])) / (vknotu[im - 1] - vknotu[ilm]);
                bu[m] += (float)(bb * (vknotu[im] - u)) / (vknotu[im] - vknotu[ilm + 1]);
            }
            bu[l] = (float)(bb * (u - vknotu[iu])) / (vknotu[iu + l - 1] - vknotu[iu]);
        }

        //calcul du point en v
        bv[1] = 1;
        int iv1 = iv + 1;
        bb = bv[1];
        bv[1] = (float)(bb * (vknotv[iv1] - v)) / (vknotv[iv1] - vknotv[iv]);
        bv[2] = (float)(bb * (v - vknotv[iv])) / (vknotv[iv1] - vknotv[iv]);

        for (int l = 3; l <= kv; l++)
        {
            bb = bv[1];
            bv[1] = (float)(bb * (vknotv[iv1] - v)) / (vknotv[iv1] - vknotv[iv - l + 2]);
            for (int m = 2; m <= l - 1; m++)
            {
                int im = iv + m;
                int ilm = iv - l + m;
                float bh = bb;
                bb = bv[m];
                bv[m] = (float)(bh * (v - vknotv[ilm])) / (vknotv[im - 1] - vknotv[ilm]);
                bv[m] += (float)(bb * (vknotv[im] - v)) / (vknotv[im] - vknotv[ilm + 1]);
            }
            bv[l] = (float)(bb * (v - vknotv[iv])) / (vknotv[iv + l - 1] - vknotv[iv]);
        }
    }

    /*private void Save()
    {
        //ofstream surf("Bspline.obj", ios::out | ios::ate);
        using (TextWriter writer = new StreamWriter("Assets\\Bspline.obj"))
        {
            for (int i = 0; i < nmaxu * nmaxv; i++)
            {
                writer.WriteLine("v {0} {1} {2}", vcal[i][0], vcal[i][1], vcal[i][2]);
            }

            for (int i = 1; i <= nmaxu * (nmaxv - 1); i++)
            {
                if (i % nmaxu != 1)
                {
                    writer.WriteLine("f {0} {1} {2}", i + nmaxu - 1, i, i + nmaxu);
                    writer.WriteLine("f {0} {1} {2}", i + nmaxu, i, i + nmaxu - 1);
                }
                if (i % nmaxu != 0)
                {
                    writer.WriteLine("f {0} {1} {2}", i + nmaxu, i, i + 1);
                    writer.WriteLine("f {0} {1} {2}", i + 1, i, i + nmaxu);
                }
            }
        }
        /*if (surf)
        {
            //surf << "mtllib Bspline.mtl" << endl;

            //ecriture des points
            for (int i = 0; i < nmaxu * nmaxv; i++)
                surf << "v " << xcal[i] << " " << ycal[i] << " " << zcal[i] << endl;

            //surf << "usemtl Material" << endl;

            //ecriture des triangles du maillage
            for (int i = 1; i <= nmaxu * (nmaxv - 1); i++)
            {
                if (i % nmaxu != 1)
                    surf << "f" << " " << i + nmaxu - 1 << " " << i << " " << i + nmaxu << endl;
                if (i % nmaxu != 0)
                    surf << "f" << " " << i + nmaxu << " " << i << " " << i + 1 << endl;
            }

            surf.close();
        }*/
    //}

    /* public Bspline()
     {
         xcontr = new float[25];
         ycontr = new float[25];
         zcontr = new float[25];

         m_Bspline = Internal_CreateBspline(xcontr, ycontr, zcontr);
     }

     ~Bspline()
     {
         Destroy();
     }

     public void Destroy()
     {
         if (m_Bspline != IntPtr.Zero)
         {
             Internal_DestroyBspline(m_Bspline);
             m_Bspline = IntPtr.Zero;
         }
     }

     public void Calc()
     {
         if (m_Bspline == IntPtr.Zero)
             throw new Exception("No native object");
         Internal_calc(m_Bspline);
     }

     [DllImport("Bspline", EntryPoint = "Internal_CreateBspline")]
     private static extern IntPtr Internal_CreateBspline(float[] x, float[] y, float[] z);

     [DllImport("Bspline", EntryPoint = "Internal_DestroyBspline")]
     private static extern void Internal_DestroyBspline(IntPtr obj);

     [DllImport("Bspline", EntryPoint = "Internal_calc")]
     private static extern void Internal_calc(IntPtr obj);*/
}
