# 배틀 로얄 짬뽕 — ProjectK

> Unity 6 탑뷰 캐주얼 배틀로얄 · K-Digital Training 12기 · Team 4  
> Reference: Escape from Duckov / SuperVive / PUBG

![thumbnail](./docs/img/thumbnail.png)

---

## 프로젝트 소개

Unity 6 엔진을 활용한 탑뷰 시점의 캐주얼 배틀로얄 게임입니다.  
플레이어 시야 제한(암흑 시야), 인벤토리/장비 슬롯, FSM 기반 게임 흐름 제어, 오브젝트 풀링을 포함한 Netcode 멀티플레이 구조를 설계·구현했습니다.

발표 당일 동일 LAN 환경에서 **20인 동시 플레이 검증** (Host 1 + Client 19)

---

## 게임 플레이

![PlayerSight_01](./docs/gifs/readme_team/PlayerSight_01.gif)![PlayerSight_02](./docs/gifs/readme_team/PlayerSight_02.gif)
**플레이어 시야 시스템** — FOV + 장애물 차폐, Stencil Buffer 기반 암흑 시야

<br>

![farming](./docs/gifs/readme_team/farming.gif)
**드롭박스 상호작용** — 아이템 획득, 장착 장비 스위칭

<br>

▶ [플레이 영상](https://youtu.be/VIDEO_ID)

---

## 실행 방법 (Windows)

1. [`ProjectK.zip`](https://github.com/Unity-Bootcamp-12/ProjectK/releases/download/%EB%B0%B0%ED%8B%80%EB%A1%9C%EC%96%84%EC%A7%AC%EB%BD%95v1.0.0/ProjectK.zip) 다운로드 후 압축 해제
2. `ProjectK.exe` 실행 (최초 실행 시 방화벽 허용)
3. 멀티플레이 *(동일 네트워크 필수)*
   - Host : `Create Host Room`
   - Client : `Join Client` → Host IP 입력
   
![lobby](./docs/img/lobby.png)
 
---

## 팀
 
| 이름 | 역할 | 담당 |
|------|------|------|
| **오융택** | 팀장 | 게임 아키텍처 설계 · 플레이어 시야 시스템 · 입력 시스템 |
| **박광호** | 팀원 | 플레이어 행동 · UI · 이펙트/사운드 시스템 · 네트워크 동기화 |
| **이정균** | 팀원 | 인벤토리 · 총기 & 총기 부착물 시스템 · 아이템 & 드롭박스 시스템 · 멀티플레이 |
| **이재훈** | 팀원 | 레벨 디자인 · QA |
 
---

## 기술 스택

- **엔진** : Unity 6 (6000.0.34f1)
- **언어** : C#
- **네트워크** : Unity Netcode for GameObjects
- **버전관리** : GitHub
- **데이터** : CSV

---

## 발표 자료

- **Download** : [ProjectK.zip](https://github.com/Unity-Bootcamp-12/ProjectK/releases/download/%EB%B0%B0%ED%8B%80%EB%A1%9C%EC%96%84%EC%A7%AC%EB%BD%95v1.0.0/ProjectK.zip)
- **Presentation** : [PDF](https://drive.google.com/file/d/1BFWQLrpwrAkx4iI_O8Isnkq3zDCFhuWF/view?usp=drive_link) · [PPTX](https://www.canva.com/design/DAGoy-LzSOc/ESZ7xtQ94T1grkF-UB3t2w/view)
- **GitHub** : [Unity-Bootcamp-12/ProjectK](https://github.com/Unity-Bootcamp-12/ProjectK)

---

## Rules

### Asset Management

모든 외부 에셋은 `DownLoadAssets/` 폴더에서 관리합니다.

- `stylize`
- `PolySamuraiPack`
- `CharacterPack`
- `polygonParticlePack`

### Coding Standard

1. [Pope Kim C# 코딩 스탠다드](https://docs.popekim.com/ko/coding-standards/csharp)
2. Unity Basic C# 스타일 가이드
   - 라이프사이클 순서 : `Awake → OnEnable/Reset → Start`
   - 직관적인 네이밍 및 컴포넌트 접근 방식 준수

### 변수 초기화 규칙

인스펙터 활성화 시 Unity가 값을 덮어쓸 수 있으므로, 필요한 경우가 아니라면 선언부 초기화를 지양합니다.

| 메서드 | 용도 |
|--------|------|
| `Awake()` | 인스턴스 생성 시점 초기화 |
| `OnEnable()` / `Reset()` | 활성화/비활성화 시 초기화 |
| `Start()` | 외부 참조 객체 초기화 및 연결 |

---

## Contact

| 이름 | 이메일 | 연락처 | GitHub |
|------|--------|--------|--------|
| **오융택** | yungtaekoh@gmail.com | 010-4810-7201 | [cyphen156](https://github.com/cyphen156) |
| **박광호** | pkh2956@naver.com | 010-6237-2956 | [doncici77](https://github.com/doncici77) |
| **이정균** | wjdrbsdl@gmail.com | 010-2933-1655 | [wjdrbsdl](https://github.com/wjdrbsdl) |
| **이재훈** | anrocksa@gmail.com | 010-2595-5993 | [LJH0521-91](https://github.com/LJH0521-91) |

공용 계정 : projectk12th@gmail.com / 010-4810-7201 / [GitHub](https://github.com/Unity-Bootcamp-12/ProjectK)