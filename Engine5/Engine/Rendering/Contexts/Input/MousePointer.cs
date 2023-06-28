using Engine.Rendering.Contexts.Input.StateStructs;

namespace Engine.Rendering.Contexts.Input;

public class MousePointer {
    public class State {
        public Point2d PixelPosition { get; private set; }
        public Point2d NdcaPosition { get; private set; }

        public State() {
            PixelPosition = new( 0, 0 );
            NdcaPosition = new( 0, 0 );
        }

        public MousePointerState ReadonlyState => new( PixelPosition, NdcaPosition );

        internal void Set( Point2d pixelPosition, Point2d ndcaPosition ) {
            PixelPosition = pixelPosition;
            NdcaPosition = ndcaPosition;
        }

        internal void Set( State state ) {
            PixelPosition = state.PixelPosition;
            NdcaPosition = state.NdcaPosition;
        }

    }

    public class MovementState {
        public readonly State CurrentPointerState;
        public readonly State PreviousPointerState;

        public MovementState() {
            CurrentPointerState = new();
            PreviousPointerState = new();
        }
    }
}
