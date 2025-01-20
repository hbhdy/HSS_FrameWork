![header](https://capsule-render.vercel.app/api?type=Rect&color=auto&height=300&section=header&text=Base%20FrameWork&fontSize=90)


<img src="https://img.shields.io/badge/unity-FFFFFF?style=for-the-badge&logo=unity&logoColor=black"/> <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white"/>  <img src="https://img.shields.io/badge/visual%20studio-%235C2D91.svg?&style=for-the-badge&logo=visual%20studio&logoColor=white" />

# 필요한 부분을 떼서 사용할 수 있도록 모듈화가 목적!

# Frame

### [GameCore](https://github.com/hbhdy/HSS_FrameWork/blob/master/DefaultFrameWork_HSS/Assets/Scripts/GameCore.cs)
- [UIManager](https://github.com/hbhdy/HSS_FrameWork/blob/master/DefaultFrameWork_HSS/Assets/Scripts/Manager/UIManager.cs) -  팝업의 온오프를 호출할 수 있는 기본 구조
- [TimeManager](https://github.com/hbhdy/HSS_FrameWork/blob/master/DefaultFrameWork_HSS/Assets/Scripts/Manager/TimeManager.cs) - 구글 시간 기반
- [SoundManager](https://github.com/hbhdy/HSS_FrameWork/blob/master/DefaultFrameWork_HSS/Assets/Scripts/Manager/SoundManager.cs) - BGM & SFX의 재생을 관리함


# Util
[ObjectPool](https://github.com/hbhdy/HSS_FrameWork/blob/master/DefaultFrameWork_HSS/Assets/Scripts/Utillity/ObjectPool.cs) - 오브젝트 풀

[InfiniteScrollView](https://github.com/hbhdy/HSS_FrameWork/blob/master/DefaultFrameWork_HSS/Assets/Scripts/Utillity/InfiniteScrollView.cs) - 재사용 스크롤 뷰 

[AlphabetUnitChange](https://github.com/hbhdy/HSS_FrameWork/blob/master/DefaultFrameWork_HSS/Assets/Scripts/Utillity/AlphabetUnitChange.cs) - double형 데이터를 알파벳 단위로 표현 <-> 알파벳 단위 텍스트를 double형으로 변환 ex) 1A, 246BC 
