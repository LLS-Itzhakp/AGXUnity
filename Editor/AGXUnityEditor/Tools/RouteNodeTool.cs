﻿using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using AGXUnity;

namespace AGXUnityEditor.Tools
{
  public class RouteNodeTool : Tool
  {
    private Func<RouteNode> m_getSelected = null;
    private Action<RouteNode> m_setSelected = null;
    private Predicate<RouteNode> m_hasNode = null;
    private Func<float> m_radius = null;
    private Func<RouteNode, Color> m_color = null;

    public ScriptComponent Parent { get; private set; }

    public RouteNode Node { get; private set; }

    public FrameTool FrameTool
    {
      get { return GetChild<FrameTool>(); }
    }

    public Utils.VisualPrimitiveSphere Visual { get { return GetOrCreateVisualPrimitive<Utils.VisualPrimitiveSphere>( "RouteNodeVisual" ); } }

    public bool Selected
    {
      get { return m_getSelected() == Node; }
      set { m_setSelected( value ? Node : null ); }
    }

    public RouteNodeTool( RouteNode node,
                          ScriptComponent parent,
                          ScriptComponent undoRedoRecordObject,
                          Func<RouteNode> getSelected,
                          Action<RouteNode> setSelected,
                          Predicate<RouteNode> hasNode,
                          Func<float> radius,
                          Func<RouteNode, Color> color )
    {
      Node = node;
      Parent = parent;
      AddChild( new FrameTool( node )
                {
                  OnChangeDirtyTarget = Parent,
                  TransformHandleActive = false,
                  UndoRedoRecordObject = undoRedoRecordObject
                } );

      m_getSelected = getSelected;
      m_setSelected = setSelected;
      m_hasNode = hasNode;
      m_radius = radius ?? new Func<float>( () => { return 0.05f; } );
      m_color = color;

      Visual.Color = m_color( Node );
      Visual.MouseOverColor = new Color( 0.1f, 0.96f, 0.15f, 1.0f );
      Visual.OnMouseClick += OnClick;
    }

    public override void OnSceneViewGUI( SceneView sceneView )
    {
      if ( Parent == null || Node == null || !m_hasNode( Node ) ) {
        PerformRemoveFromParent();
        return;
      }

      float radius = ( Selected ? 3.01f : 3.0f ) * m_radius();
      Visual.Visible = !EditorApplication.isPlaying;
      Visual.Color = Selected ? Visual.MouseOverColor : m_color( Node );
      Visual.SetTransform( Node.Position, Node.Rotation, radius, true, 1.2f * m_radius(), Mathf.Max( 1.5f * m_radius(), 0.25f ) );
    }

    private void OnClick( AGXUnity.Utils.Raycast.Hit hit, Utils.VisualPrimitive primitive )
    {
      Selected = true;
    }
  }
}
