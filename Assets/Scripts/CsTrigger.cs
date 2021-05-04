using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsTrigger : MonoBehaviour
{

    struct Agent
    {
        public Vector2 position;
        public float angle;
    };

    public ComputeShader ComputeShader;


    public int TextureWidth;
    public int TextureHeight;
    public int NumberOfAgents;
    public float MoveSpeed;
    public float EvaporateSpeed;
    public float DiffuseSpeed;
    public float SensorOffsetDst;
    public float SensorAngleSpacing;
    public float TurnSpeed;
    public float SensorSize;
    public Material Material;
    
    private RenderTexture renderTexture;


    private Agent[] initAgentsData()
    {
        Agent[] agents = new Agent[NumberOfAgents];
        for(int i = 0; i < agents.Length; i++)
        {
            Agent a = new Agent();
            a.angle = Random.Range(0, 360);
            a.position = new Vector2(TextureWidth / 2, TextureHeight / 2);
            agents[i] = a;
        }
        return agents;
    }

    private RenderTexture CreateTexture(int x, int y, int z)
    {
        RenderTexture rt;
        rt = new RenderTexture(x, y, z);
        rt.filterMode = FilterMode.Point;
        rt.antiAliasing = 4;
        rt.enableRandomWrite = true;
        rt.Create();
        return rt;
    }

    // Start is called before the first frame update
    void Start()
    {

        ComputeBuffer agentsBuffer = new ComputeBuffer(NumberOfAgents, sizeof(float) * 3);
        agentsBuffer.SetData(initAgentsData());

        renderTexture = CreateTexture(TextureWidth, TextureHeight, 1);

        ComputeShader.SetInt("TextureWidth", TextureWidth);
        ComputeShader.SetInt("TextureHeight", TextureHeight);
        ComputeShader.SetInt("NumAgents", NumberOfAgents);
        ComputeShader.SetFloat("PI", Mathf.PI);
        ComputeShader.SetFloat("MoveSpeed", MoveSpeed);
        ComputeShader.SetFloat("EvaporateSpeed", EvaporateSpeed);
        ComputeShader.SetFloat("DiffuseSpeed", DiffuseSpeed);
        ComputeShader.SetFloat("SensorOffsetDst", SensorOffsetDst);
        ComputeShader.SetFloat("SensorAngleSpacing", SensorAngleSpacing);
        ComputeShader.SetFloat("TurnSpeed", TurnSpeed);
        ComputeShader.SetFloat("SensorSize", SensorSize);
        ComputeShader.SetTexture(0, "Result", renderTexture);
        ComputeShader.SetTexture(1, "Result", renderTexture);
        ComputeShader.SetBuffer(0, "Agents", agentsBuffer);

        
        //Graphics.Blit(renderTexture, DestinationTexture);
    }


    public void Update()
    {
        ComputeShader.SetFloat("DeltaTime", Time.deltaTime);
        ComputeShader.Dispatch(0, NumberOfAgents, 160, 1);
        ComputeShader.Dispatch(1, TextureWidth / 4, TextureHeight / 4, 1);
        Material.mainTexture = renderTexture;
    }
}
