# 배틀로얄짬뽕 — ProjectK

탑뷰 배틀로얄에서 과도하게 노출되는 전장 정보를 제한하고,  
멀티플레이 환경에서 입력이 올바른 대상에게만 적용되도록 제어하는 시스템을 구현한 프로젝트입니다.

![thumbnail](./ProjectK/docs/img/thumbnail.png)

> Unity 6 탑뷰 캐주얼 배틀로얄 · K-Digital Training 12기 · Team 4  
> Reference: Escape from Duckov / SuperVive / PUBG

---

## 프로젝트 개요

> 제한된 시야 속에서 탐색과 교전을 반복하는 캐주얼 탑뷰 배틀로얄

Unity 6 기반 탑뷰 캐주얼 배틀로얄 게임입니다.  
제한된 시야 속에서 아이템을 파밍하고, 탐색과 교전을 반복하며 마지막까지 생존하는 것이 목표입니다.

발표 당일 동일 LAN 환경에서 **20인 동시 플레이를 검증**했습니다. (Host 1 + Client 19)

- **팀 구성** : **오융택(팀장/본인)**, 박광호(팀원), 이정균(팀원), 이재훈(팀원)
- **개발 기간** : 2025.05.12 ~ 2025.05.29 (18일)
- **팀 레포지토리** : [Unity-Bootcamp-12/ProjectK](https://github.com/Unity-Bootcamp-12/ProjectK)
- **포트폴리오** : [[Unity6] 배틀로얄짬뽕 - ProjectK](https://cyphen156.tistory.com/462)

---

## 수행 역할

게임 아키텍처 설계를 주도하고, 플레이어 시야 시스템과 입력 처리 구조를 담당했습니다.

- 플레이어 시야 시스템
  - Raycast 기반 FOV 메시 생성
  - Stencil Buffer 기반 시야 필터링
  - 클라이언트 단위 시야 처리
- 입력 시스템
  - IPlayerInputReceiver 기반 입력 대상 분리
  - InputReceiver enum 기반 입력 범위 제어
  - 플레이어 상태 기반 입력 처리 제한
- 크로스헤어 & 탄착군 시스템
  - 플레이 상태 기반 크로스헤어 동적 크기 계산
  - 탄착군 범위와 시각 피드백 연동

---

## 주요 시스템

### 플레이어 시야 시스템

플레이어 기준 시야 범위를 계산하고, 시야 밖 전투 정보를 화면에서 제한하는 시스템입니다.

| 플레이어 시야 | 전투 정보 차단 |
|:---:|:---:|
| ![unblocked](./ProjectK/docs/gifs/readme_team/PlayerSight_Unblocked.gif) | ![blocked](./ProjectK/docs/gifs/readme_team/PlayerSight_Blocked.gif) |

#### FOV 메시 생성

플레이어의 가시 영역을 실시간으로 계산하는 기능입니다.
방향성 FOV와 근접 FOV를 결합해 시야를 구성합니다.

- 마우스 방향 기준 Raycast를 방사형으로 발사해 방향성 FOV 계산
- 이진 탐색으로 경계 구간을 보완해 벽면을 따라 자연스러운 시야 형성
- 플레이어 주변 고정 반경을 원형 메시로 근접 시야 형성

```csharp
// PlayerSight.cs — DrawFieldOfView()
for (int i = 0; i <= stepCount; ++i)
{
    float angle = baseAngle - activeViewAngle / 2f + angleStep * i;
    ViewCastInfo newViewCast = ViewCast(angle);
 
    bool edgeDistanceThresholdExceeded =
        Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
 
    if (oldViewCast.hit != newViewCast.hit ||
       (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
    {
        EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
        viewPoints.Add(edge.pointA);
        viewPoints.Add(edge.pointB);
    }
    viewPoints.Add(newViewCast.point);
    oldViewCast = newViewCast;
}
```
 
#### Stencil Buffer 기반 오브젝트 필터링
 
FOV 메시로 가시 영역을 계산하고, Stencil Buffer로 해당 영역을 마킹해 시야 밖 전투 정보를 필터링합니다.  
지형은 항상 보이되, 전투 정보만 선택적으로 차단됩니다.
 
```
StencilMask.shader    →  FOV 메시 영역에 Stencil 값 기록 (색상 출력 없음)
StencilCover.shader   →  Stencil 값이 기록된 영역만 URP Lit으로 재렌더링
StencilFilter.shader  →  Stencil 값이 없는 영역 전체에 오버레이 적용
```
 
- 렌더링 단계에만 적용되어 멀티플레이 동기화 구조와 분리
- `LateUpdate()` 에서 `IsOwner` 체크 후 클라이언트 단위로 독립 처리
- 시야 밖 오브젝트의 그림자는 유지 — 완전 정보 차단 대신 최소한의 위치 힌트로 활용

---

### 입력 시스템

플레이어 행동을 `IPlayerInputReceiver`로 정의하고, `InputManager`가 로컬 플레이어를 식별해 입력을 전달합니다.

- 입력 수신 대상은 인터페이스 기반으로 분리
- 입력 허용 범위는 게임 상태에 따라 전환
- 플레이어 상태에 따라 입력 처리 여부 제어

#### IPlayerInputReceiver

```csharp
public interface IPlayerInputReceiver
{
    void InputMove(MoveType inMoveType, float inH, float inV);
    void InputAttack();
    void InputReload();
    void InputMousePosition(Vector3 inMousePosition);
    void InteractDropBox();
    void Dodge();
    void IsAim(bool isAim);
    void UseItem(int number);
    void UseGranade();
}
```

- 로컬 플레이어 등록 시 `OnLocalPlayerRegistered` 이벤트로 브로드캐스트
- `PlayerUIManager`, `InGameUIManager` 등 관심 객체가 PlayerController 참조를 공유

#### InputReceiver enum 기반 입력 범위 제어

```csharp
public enum InputReceiver
{
    None,
    PlayerOnly,
    InGameUIOnly,
    All
}
```

- 플레이어 등록 시점 → `InGameUIOnly`
- `GameState.Play` 이후 → `All` 로 전환
- `PlayerState.Die` 시 입력 차단

#### 상태 기반 플레이어 제어 (PlayerStateMachine)

플레이어 행동을 `Idle / Walk / Dodge / Die` 상태로 관리합니다.

- 상태 전이는 중앙에서 관리, `isStateLock` 으로 상태 충돌 방지
- Dodge 상태는 코루틴 기반 자동 복귀, 상태 진입 시 무적 처리

```csharp
// TakeDamageRpc() — Dodge 상태 무적
if (netCurrentPlayerState.Value == PlayerState.Dodge) return;
```

---

### 크로스헤어 & 탄착군 시스템

이동 상태, 조준 여부, 부착물 스탯을 반영해 크로스헤어 크기를 동적으로 계산하고, 탄착군 범위와 연동합니다.

```csharp
// PlayerController.cs — UpdateCrosshairSize()
float targetCrosshairSize = defaultCrosshairSize;

if (currentMoveType == MoveType.Run)
    targetCrosshairSize += crosshairspreadRadius;       // 달리기 → 확대

if (playerGun != null)
    targetCrosshairSize += playerGun.equiptFocusRegion; // 부착물 Focus 스탯 반영

if (isAimed)
    targetCrosshairSize -= crosshairspreadRadius;       // 조준(심호흡) → 축소

currentCrosshairSize = Mathf.Lerp(
    currentCrosshairSize, targetCrosshairSize, Time.deltaTime * crosshairLerpSpeed);

OnCrosshairSizeChanged?.Invoke(currentCrosshairSize);
```

| 달리기 | 평시 | 심호흡 (조준) |
|:---:|:---:|:---:|
| ![expanded](./ProjectK/docs/gifs/readme_private/bulletSpread_expanded.gif) | ![default](./ProjectK/docs/gifs/readme_private/bulletSpread_Default.gif) | ![zoomed](./ProjectK/docs/gifs/readme_private/bulletSpread_zoomed.gif) |

---

## 트러블슈팅

### 1. 비정상적인 FOV 메시 생성

| | |
|---|---|
| **문제** | 시야 메시가 불규칙하게 찢어지거나, 비정상적인 가시 영역이 생성되는 현상 발생 |
| **원인** | 불규칙한 터레인 표면과 다양한 오브젝트 콜라이더에 의해 Raycast 거리 값이 불연속적으로 수집되면서, Edge 간 연결이 깨지고 메시가 비정상적으로 생성됨 |
| **대응** | Raycast 발사 기준점을 캐릭터 발이 아닌 가슴 높이로 조정해 지면 및 저높이 콜라이더와의 충돌을 줄이고, 일부 오브젝트의 콜라이더를 단순화 및 통합해 거리 값 급변을 완화 |
| **결과** | 비정상적인 가시 영역 생성이 대부분 완화됨 |
| **한계** | 서로 다른 오브젝트에 hit하면서 발생하는 거리 불연속은 근본적으로 해소되지 않아, 특정 환경에서는 동일 현상이 재발할 수 있음 |

### 2. 플레이어 회전 기준 오류 및 크로스헤어 - 탄 방사 범위 불일치

| | |
|---|---|
| **문제** | 캐릭터가 월드 Y축이 아닌 카메라 축을 기준으로 회전하고, 크로스헤어 범위와 실제 탄 방사 범위가 일치하지 않는 문제 발생 |
| **원인** | 기울어진 탑뷰 카메라 환경에서 `ScreenToWorldPoint()` 를 사용할 경우, 카메라 앞의 특정 평면을 기준으로 좌표가 해석되어 방향 벡터가 카메라 기준으로 왜곡됨 |
| **대응** | 마우스 좌표의 z값을 카메라 높이로 고정해 조준점을 일정 평면 기준으로 계산하도록 보정하고, 방향 벡터에 `direction.y = 0f` 를 적용해 캐릭터가 월드 평면 기준으로만 회전하도록 수정 |
| **결과** | 캐릭터가 월드에 수직으로 선 상태에서 마우스 방향으로 회전하도록 개선, 크로스헤어와 탄 방사 범위 간 불일치도 플레이 가능한 수준으로 완화 |
| **한계** | 카메라 기울기에 따른 투영 왜곡을 근본적으로 제거한 것은 아니므로, 특정 위치에서는 여전히 불일치가 발생함 |

---

## 사후 개선점

### 서버 권한 기반 시야 제한

현재 Host-Client 구조에서는 렌더링 단계에서만 시야를 제한합니다.  
시야 밖 오브젝트 정보가 클라이언트 메모리에 존재하기 때문에 핵 클라이언트로 정보를 읽는 것을 막을 수 없습니다.  
데디케이티드 서버 구조에서는 서버가 시야 밖 정보를 전송하지 않는 방식으로 근본적인 해결이 가능합니다.

### 탄착군 정규화 공식 개선

탄착군 각도 계산에 고정 배율(`/ 10`)을 사용해 총기 스탯 변화에 따른 변화가 선형적이지 않습니다.  
`viewDistance` 와 총기 스탯을 함께 반영한 정규화 공식으로 개선이 필요합니다.

### 장애물 상호작용 개선

자신 소유의 오브젝트에 대한 사격/피격 방지 로직으로 인해, 플레이어가 설치한 장애물은 본인의 공격으로 피해를 입지 않습니다.  
시야 차단 구조와 결합되면서, 설치자는 장애물을 파괴할 수 없지만 상대는 공격을 통해 파괴 가능한 비대칭 상황이 발생합니다.

조정 방향 두 가지를 검토 중입니다.

- 자신 소유의 장애물은 가시 시야 제공 + 자기 장애물은 피격 제외 유지
- 시야 차단 유지 + 자기 장애물 파괴 가능으로 변경


---

## 기술 스택

| 항목 | 내용 |
|---|---|
| 엔진 | Unity 6 (6000.0.34f1) |
| 언어 | C# |
| 네트워크 | Unity Netcode for GameObjects |
| 버전관리 | GitHub |
| 데이터 | CSV |
