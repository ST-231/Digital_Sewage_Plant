# 数字化模拟污水处理系统

该系统为基于unity开发的数字孪生污水处理厂，对真实污水处理厂进行 1:1 三维可视化建模，并集成数学模拟算法，实现从预处理到污泥处置的全流程仿真与参数调优。

---

## 核心功能

1. 全流程三维可视化 

* **真实建模**：根据卫星地图与实地考察数据，对粗格栅、AAO池、二沉池、深床滤池、紫外消毒池等核心工艺单元进行全要素建模 。
<img width="1825" height="1027" alt="sewage_plant" src="https://github.com/user-attachments/assets/42a1ee6f-409d-4142-8199-fa17d70d0c09" />

* **多视角导览**：支持第一人称漫游与上帝视角自动巡检，结合任务导航系统推送节点信息 。
<img width="960" height="540" alt="process" src="https://github.com/user-attachments/assets/86b7e50d-c646-4834-9b5f-86daf5e7c725" />

* **设备拆解**：通过动态交互展示复杂设备内部结构，便于直观理解工艺构造 。


2. 数学建模与仿真计算 

系统内置完整的污水处理动力学模型，支持用户调节参数并实时观察输出水质变化：

* **AAO 工艺模拟**：包含 BOD5 降解（一级动力学）、生物脱氮（温度修正）及生物除磷模型 。

* **二沉池与深度处理**：模拟计算 MLSS 浓度、表面负荷、污泥回流比以及粪大肠菌群灭活率等关键指标 。

* **经验公式拟合**：根据搜集到的工业数据进行公式拟合，反映设备处理能力与流量损失率 。
<img width="750" height="378" alt="formula" src="https://github.com/user-attachments/assets/f380e9ff-4eb5-4464-bdbb-202915fb8b0b" />


3. 运维辅助与成本评估 

* **动态调优**：快速计算不同参数组合下的 COD 去除率、脱氮效率等，避免实体设备反复试错 。

* **能耗预估**：综合考虑进水流量、曝气能耗及峰谷电价，生成处理成本预估报表 。
<img width="960" height="540" alt="result" src="https://github.com/user-attachments/assets/73030ce1-69eb-4eb6-b31d-de32fe722b95" />
