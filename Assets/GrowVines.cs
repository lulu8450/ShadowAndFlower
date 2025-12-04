using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GrowVines : MonoBehaviour
{
    public List<MeshRenderer> growVinesMeshes;
    public float timeToGrow = 5f;
    public float refreshRate = 0.05f;
    [Range(0, 1)]
    public float minGrow = 0.2f;
    [Range(0,1)]
    public float maxGrow = 0.97f;

    public List<Material> growVinesMaterials = new List<Material>();
    public bool fullyGrown;

    [Header ("EmissiveStrenght")]
    public float emissiveStrenght;
    public int maxEmissiveStrenght = 100;
    public int minEmissiveStrenght = 25;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i=0; i>growVinesMaterials.Count; i++)
        {
            for (int j = 0; j < growVinesMeshes[i].materials.Length; j++)
            {
                if (growVinesMeshes[i].materials[j].HasProperty("Grow_"))
                {
                    growVinesMeshes[i].materials[j].SetFloat("Grow_", minGrow);
                    growVinesMaterials.Add(growVinesMeshes[i].materials[j]);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SpaceKey On");
            for(int i=0; i<growVinesMaterials.Count; i++)
            {
                StartCoroutine(GrowVine(growVinesMaterials[i]));
                StartCoroutine(EmissiveVine(growVinesMaterials[i]));
            }
        }
    }

    IEnumerator GrowVine (Material mat)
    {
        float growValue = mat.GetFloat("Grow_");

        if(!fullyGrown)
        {
            while(growValue < maxGrow)
            {
                growValue += 1 /(timeToGrow/refreshRate);
                mat.SetFloat("Grow_", growValue);

                if(emissiveStrenght > minEmissiveStrenght) emissiveStrenght --;
                growVinesMeshes[0].material.SetFloat("EmissiveStrength_", emissiveStrenght);
                yield return new WaitForSeconds(refreshRate);
            }
        }
        else
        {
            while (growValue > minGrow)
            {
                growValue -= 1 / (timeToGrow / refreshRate);
                mat.SetFloat("Grow_", growValue);

                if (emissiveStrenght < maxEmissiveStrenght) emissiveStrenght ++;
                growVinesMeshes[0].material.SetFloat("EmissiveStrength_", emissiveStrenght);
                yield return new WaitForSeconds(refreshRate);
            }
        }
        if (growValue >= maxGrow)
            fullyGrown = true;
        else
            fullyGrown = false;
    }

    IEnumerator EmissiveVine(Material mat)
    {
        float growValue = mat.GetFloat("Grow_");

        if (!fullyGrown)
        {
            while (growValue < maxGrow)
            {
                if (emissiveStrenght > minEmissiveStrenght) emissiveStrenght--;
                growVinesMeshes[0].material.SetFloat("EmissiveStrength_", emissiveStrenght);
                yield return new WaitForSeconds(refreshRate);
            }
        }
        else
        {
            while (growValue > minGrow)
            {
                if (emissiveStrenght < maxEmissiveStrenght) emissiveStrenght++;
                growVinesMeshes[0].material.SetFloat("EmissiveStrength_", emissiveStrenght);
                yield return new WaitForSeconds(refreshRate);
            }
        }
        if (growValue >= maxGrow)
            fullyGrown = true;
        else
            fullyGrown = false;
    }
}


// Création Vines
// Quand les vines apparaissent, faire en sorte que l'émission soit forte (a fond) pour que ca merge avec le VFX 
// et ensuite quand VFX termine diminuer l'émission des deux etc.