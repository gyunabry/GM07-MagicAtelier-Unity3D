# [Unity3D] Magic Atelier
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/9cae8097-161d-427f-b541-d45ed20a58d2" />

**MagicAtelier**는 Unity 6와 C#으로 진행한 3D 탑다운뷰 타이쿤 게임 프로젝트입니다.

  
## 목차
- [게임 소개](#1-게임-소개)
- [핵심 기능 및 특징](#2-핵심-기능-및-특징)
- [개발 환경 및 기술 스택](#3-개발-환경-및-기술-스택)
- [팀 구성 및 담당](#4-팀-구성-및-담당)

  
## 1. 게임 소개

<table align="center">
  <tr>
    <td align="center">
      <img src="https://github.com/user-attachments/assets/c4ff8c7d-549a-45c5-aae9-4bf5d52929c2" width="100%">
      <br>
      <b>건물 배치 시스템</b>
      <br>
    </td>
    <td align="center">
      <img src="https://github.com/user-attachments/assets/d739ed01-7d79-4d14-a084-053128036a54" width="100%">
      <br>
      <b>스킬 트리</b>
      <br>
    </td>
  </tr>
  <tr>
    <td align="center">
      <img src="https://github.com/user-attachments/assets/f67d4ddc-e4da-447e-8e1b-484e81c354b7" width="100%">
      <br>
      <b>재료 수급 사냥터</b>
      <br>
    </td>
    <td align="center">
      <img src="https://github.com/user-attachments/assets/8097faf3-5e4c-413d-903f-1611ad1d96d4" width="100%">
      <br>
      <b>직원 고용을 통한 자동화</b>
      <br>
    </td>
  </tr>
</table>

플레이어는 적을 처치해 재료를 획득하고, 얻은 재료를 가공해 아이템을 만들어 손님에게 판매합니다.  
사냥과 장사를 진행하며 플레이어를 돕는 일꾼들을 고용해 간편하게 게임을 진행할 수 있습니다.  
스킬트리 화면에서 플레이어를 강화하고 공방 운영에 도움을 주는 스킬을 습득하거나 강화할 수 있습니다.

* **개발 기간**: 2026.08.04 ~ 2026.09.04
* **개발 인원**: 5명
* **플랫폼**: Windows
* **플레이 인원**: 1인

  
## 2. 핵심 기능 및 특징
### [시설 배치 시스템]
Unity Grid 기반으로 시설의 위치와 방향을 미리 검사한 뒤 공방에 배치할 수 있습니다.  
배치된 시설은 다시 이동하거나 판매할 수 있습니다.  

  
### [레시피 기반 자동 생산]
생산 시설에 레시피를 선택하고 알맞은 재료 아이템을 납품하면 레시피에 따른 시간마다 아이템을 생산합니다.  
운반 직원을 배치해 해당 과정을 자동화할 수 있습니다.  

  
### [공용 인벤토리]
플레이어 인벤토리, 시설별 인벤토리, 직원 화물 인벤토리처럼 여러 시스템에서 사용가능한 인벤토리입니다.  
해당 인벤토리를 사용하는 오브젝트는 아이템 추가, 다른 인벤토리로의 아이템 전송 등 동일 규칙을 공유합니다.  
모든 판매대는 하나의 인벤토리를 공유합니다.  

  
### [공방 확장 및 사냥터 해금]
재화와 진행 조건을 충족하면 공방의 사용 가능한 공간을 확장할 수 있습니다.  
사냥터도 동일하게 조건을 충족하면 재화를 지불해 해금할 수 있고, 해금한 사냥터에 따라 레시피도 같이 해금됩니다.  

  
### [게임 데이터 저장 및 복구]
플레이어 정보, 시설 배치, 인벤토리, 진행 상황 등의 데이터를 저장하고 불러올 수 있습니다.  

  
### [시설 상호작용]
플레이어는 공방에 배치된 시설과 상호작용해 생산, 판매, 분해 등의 기능을 이용할 수 있습니다.  
시설별 상호작용시 표시되는 UI를 라우터를 통해 표시합니다.  

  
### [손님 시스템]
손님은 공방을 방문해 해금된 레시피를 기반으로 주문을 요청합니다.  
상황에 따라 NavMesh 기반 이동, 대기, 구매 등의 행동을 수행합니다.
공방에 일정 시간동안 머무르다가 인내심이 바닥나면 공방을 벗어납니다.  

  
### [직원 시스템]
각 작업에 특화된 직업을 직원 시설에서 고용하고 배치할 수 있습니다.  
직원은 지정된 업무를 반복 수행해 공방 운영을 자동화합니다.  

  
### [아이템 판매 기능]
판매대가 설치되어 있고 대기 중인 손님이 있을 때, 플레이어나 직원은 상호작용을 통해 재고가 있는 아이템을 판매할 수 있습니다.
플레이어는 판매된 아이템에 따라 경험치와 골드를 획득합니다.  

  
### [스킬트리 시스템]
사냥과 공방 운영에 도움이 되는 스킬을 습득하고 강화할 수 있습니다.    


### [몬스터 AI]
상태 머신과 NavMesh를 기반으로 대기와 배회를 반복하며, 피격 시 플레이어의 반대 방향으로 도주합니다.  


### [플레이어 자동 공격]
플레이어가 공격 스킬을 장착하면 공격 범위 안의 적을 자동으로 탐색하고 공격합니다.  
최대 3개의 공격 스킬을 동시에 사용할 수 있습니다.    


## 3. 개발 환경 및 기술 스택
* **게임 엔진**: Unity6000.3.7f1
* **언어**: C#
* **형상 관리**: Git, GitHub
* **QA**: Jira
* **협업**: Notion, Discord

## 4. 팀 구성 및 담당
|프로필|이름|담당 기능 및 역할|GitHub|
|:---:|:---:|:---:|:---:|
|<img src="https://avatars.githubusercontent.com/u/172166404?v=4" width="100">|구한빈(팀장)|시설 배치, 인벤토리, 시설 시스템, 공방 확장, 세이브/로드|[@gyunabry](https://github.com/gyunabry)|
|<img src="https://avatars.githubusercontent.com/u/200070976?v=4" width="100">|김나연|기획, UI/UX|[@Gachathief](https://github.com/Gachathief)|
|<img src="https://avatars.githubusercontent.com/u/285566128?v=4" width="100">|남현재|시스템 기획, QA|[@skaguswoxx](https://github.com/skaguswoxx)|
|<img src="https://avatars.githubusercontent.com/u/68463308?v=4" width="100">|전재창|손님 시스템, 직원|[@JaeChang](https://github.com/wockd9600)|
|<img src="https://avatars.githubusercontent.com/u/279920847?v=4" width="100">|최겸|전투 시스템, 스킬트리 시스템, 몬스터 AI|[@Ritz55555](https://github.com/Ritz55555)|
