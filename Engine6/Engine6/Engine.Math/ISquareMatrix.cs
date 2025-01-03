﻿using System;

namespace Engine;

public interface ISquareMatrix<TMatrix> : IInvertible<TMatrix>
	where TMatrix :
		unmanaged, ISquareMatrix<TMatrix> {
	TMatrix GetTransposed();
}
