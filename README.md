# ProjectK
Unity6 12th TeamProject Team 4th ProjectK  
**Reference**: SuperVive

---

## 📬 Contact Us
- **공용 계정**: projectk12th@gmail.com / 010-4810-7201 / [GitHub](https://github.com/Unity-Bootcamp-12/ProjectK)
- **오융택**: yungtaekoh@gmail.com / 010-4810-7201 / [GitHub](https://github.com/cyphen156)
- **이정균**: wjdrbsdl@gmail.com / 010-2933-1655 / [GitHub](https://github.com/wjdrbsdl)
- **박광호**: pkh2956@naver.com / 010-6237-2956 / [GitHub](https://github.com/doncici77)
- **이재훈**: anrocksa@gmail.com / 010-2595-5993 / [GitHub](https://github.com/LJH0521-91)

---

## 📚 Rules

### 📁 Asset Management
모든 외부 에셋은 아래 폴더에 저장하여 관리합니다:

- `DownLoadAssets/`  
  *(추가된 에셋 목록은 여기에 작성)*

---

### 🧾 Coding Standard
코딩 스탠다드는 다음 두 가지 기준을 따릅니다:

1. [Pope Kim의 C# 코딩 스탠다드](https://docs.popekim.com/ko/coding-standards/csharp)
2. **Unity Basic C# 스타일 가이드**  
   - Unity 라이프사이클 함수 순서 (`Awake → OnEnable/Reset → Start`)  
   - 직관적인 네이밍 및 컴포넌트 접근 방식 준수

---

### ⚙️ 변수 초기화 규칙

인스펙터 활성화 시 Unity가 값을 덮어쓸 수 있으므로, **필요한 경우가 아니라면 선언부에서 변수 초기화를 지양**합니다. 
초기화는 아래 기준을 따릅니다:

1. `Awake()`  
   - **인스턴스 생성 시점에서 초기화**
2. `OnEnable()` / `Reset()`  
   - **활성화/비활성화 시 초기화 시 사용**
3. `Start()`  
   - **외부 참조 객체 초기화 및 연결용**

---
