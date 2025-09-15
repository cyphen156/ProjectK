# ProjectK

> Unity6 12th **TeamProject · Team 4**  
> Reference: **SuperVive / PUBG**

## 프로젝트 소개
### Unity6 엔진을 활용한 **탑뷰 시점의 캐주얼 배틀로얄** 게임입니다.  
플레이어 **시야 제한(암흑 시야)**, **인벤토리/장비 슬롯**, **FSM 기반 게임 흐름 제어**, **오브젝트 풀링을 포함한 Netcode 멀티플레이** 구조를 설계·구현했습니다.  
싱글에서 **로컬/근거리 네트워크 멀티플레이**로 확장했으며, **데이터/정책 분리**와 **컴포넌트 단위 개발**로 기능 교체 및 테스트 효율을 높였습니다.  
- **팀 구성** : 오융택(팀장), 박광호, 이정균, 이재훈  
- **개발 기간** : 2025.05.12 ~ 2025.05.29 (18일)  
- **기획 의도** : Unity Netcode를 활용한 **배틀로얄 멀티플레이 게임 개발**  
- **개발 목표**
  - **Unity Netcode** 기반 **Host–Client 멀티플레이** 구조 구현
  - **플레이어 시야 제한**으로 긴장감 있는 전투 연출
  - **인벤토리/장비 슬롯** 시스템 구축
  - **엄폐물 배치**로 전략성 강화
  - **FSM**으로 전체 게임 흐름 제어
- **시연 성과** : 발표 당일 동일 LAN 환경에서 **20인 동시 플레이** 검증 (Host 1 + Client 19)

## 핵심 기능
- **멀티플레이(Netcode)** : Host 기준 이동·상태 동기화, 시스템 이벤트 브로드캐스팅
- **플레이어 시야 제한** : Raycast 탐지 + 마스킹
- **플레이어 FSM** : 체력/장비/버프 등 상태 관리 및 UI 반영
- **총기 FSM** : 사격/재장전/쿨타임 상태 전이
- **아이템 파밍/사용** : 랜덤 드롭박스 파밍, **소비형/장착형** 처리
- **인벤토리·총기 부착물 슬롯** : 슬롯–아이템 매핑, UI 동기화
- **오브젝트 풀링** : 총알/이펙트 재사용으로 성능 안정화

## 수행 역할
여기에 역할 명시 추가

## 실행 방법 (Windows 빌드)
1. `ProjectK.zip` 다운로드  
2. 압축 해제 → `ProjectK.exe` 실행 (최초 실행 시 방화벽 허용)  
3. 게임 플레이 : PC1에서 **Host** 시작 → PC2에서 **Client** 접속 *(동일 네트워크 필수)*

## 기술 스택
- **엔진** : Unity 6 (6.0.34f1)  
- **언어** : C#  
- **네트워크** : Unity Netcode for GameObjects  
- **버전관리** : GitHub Desktop  
- **데이터** : CSV

## 시연 자료
- **프로젝트(배포)** : [`ProjectK.zip`](https://github.com/Unity-Bootcamp-12/ProjectK/releases/download/%EB%B0%B0%ED%8B%80%EB%A1%9C%EC%96%84%EC%A7%AC%EB%BD%95v1.0.0/ProjectK.zip)
- **Presentation**
  - [`배틀로얄짬뽕_4조.pdf`](https://drive.google.com/file/d/1BFWQLrpwrAkx4iI_O8Isnkq3zDCFhuWF/view?usp=drive_link)
  - [`배틀로얄짬뽕_4조.pptx`](https://docs.google.com/presentation/d/16wLXi8FHvrB7dzUwHuOSS3B_d_yf-O84/edit?usp=drive_link&ouid=114643264030116262200&rtpof=true&sd=true)

## 저장소
- **GitHub** : https://github.com/cyphen156/ProjectK

---

## 📬 Contact Us
- **공용 계정** : projectk12th@gmail.com / 010-4810-7201 / [GitHub](https://github.com/Unity-Bootcamp-12/ProjectK)  
- **오융택** : yungtaekoh@gmail.com / 010-4810-7201 / [GitHub](https://github.com/cyphen156)  
- **이정균** : wjdrbsdl@gmail.com / 010-2933-1655 / [GitHub](https://github.com/wjdrbsdl)  
- **박광호** : pkh2956@naver.com / 010-6237-2956 / [GitHub](https://github.com/doncici77)  
- **이재훈** : anrocksa@gmail.com / 010-2595-5993 / [GitHub](https://github.com/LJH0521-91)

## Rules
### Asset Management
모든 외부 에셋은 아래 폴더에 저장하여 관리합니다:
- `DownLoadAssets/`  
  1. stylize  
  2. PolySamuraiPack  
  3. CharacterPack  
  4. polygonParticlePack

### Coding Standard
코딩 스탠다드는 다음 기준을 따릅니다:
1. [Pope Kim의 C# 코딩 스탠다드](https://docs.popekim.com/ko/coding-standards/csharp)  
2. **Unity Basic C# 스타일 가이드**  
   - Unity 라이프사이클 순서 (`Awake → OnEnable/Reset → Start`)  
   - 직관적인 네이밍 및 컴포넌트 접근 방식 준수

### 변수 초기화 규칙
인스펙터 활성화 시 Unity가 값을 덮어쓸 수 있으므로, **필요한 경우가 아니라면 선언부 초기화를 지양**합니다.  
초기화는 아래 기준을 따릅니다:
1. `Awake()` — **인스턴스 생성 시점 초기화**  
2. `OnEnable()` / `Reset()` — **활성화/비활성화 시 초기화**  
3. `Start()` — **외부 참조 객체 초기화 및 연결**
