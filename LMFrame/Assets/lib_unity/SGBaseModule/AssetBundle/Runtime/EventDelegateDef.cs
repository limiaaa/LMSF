using UnityEngine;

/// <summary>
/// Event + return [Type] + params [Type] + [Type] + ...
/// </summary>

//EventDelegate.
public delegate void ED_Void();
public delegate void ED_Void_I(int i);
public delegate void ED_Void_B(bool b);
public delegate void ED_Void_F(float b);
public delegate void ED_Void_S(string str);
public delegate bool ED_Bool_I(int i);
public delegate void ED_Void_Go(GameObject go);
public delegate void ED_Void_Go_I(GameObject go, int i);

public delegate T ED_0Param_Return<T>();
public delegate T ED_1Param_Return<T, P1>(P1 p1);
public delegate T ED_2Param_Return<T, P1, P2>(P1 p1, P2 p2);
public delegate T ED_3Param_Return<T, P1, P2, P3>(P1 p1, P2 p2, P3 p3);
public delegate T ED_4Param_Return<T, P1, P2, P3, P4>(P1 p1, P2 p2, P3 p3, P4 p4);

public delegate void ED_1Param<P1>(P1 p1);
public delegate void ED_2Param<P1, P2>(P1 p1, P2 p2);
public delegate void ED_3Param<P1, P2, P3>(P1 p1, P2 p2, P3 p3);
public delegate void ED_4Param<P1, P2, P3, P4>(P1 p1, P2 p2, P3 p3, P4 p4);