using System;
using System.Drawing;

namespace Game.Component
{
	public struct ColorPolygon : ECS.IComponentData
	{
		public SolidBrush brush;
		public PointF[] points;
		
		public ColorPolygon ARGB(byte a, byte r, byte g, byte b){ brush.Color = Color.FromArgb(a,r,g,b); return this; }
		
		public static ColorPolygon Create(byte a, byte r, byte g, byte b, params Vector2[] args)
		{
			PointF[] ps = new PointF[args.Length];
			for(int i=0; i<ps.Length; i++)
			{
				ps[i].X = args[i].x;
				ps[i].Y = args[i].y;
			}
			
			return new ColorPolygon
			{
				brush = new SolidBrush(Color.FromArgb(a,r,g,b)),
				points = ps
			};
		}
		
		public static ColorPolygon Default()
		{
			Vector2[] vertices = new Vector2[3];
			vertices[0] = new Vector2(-0.5f, -0.5f);
			vertices[1] = new Vector2(-0.5f, 0.5f);
			vertices[2] = new Vector2(0.5f, -0.5f);
			
			return Create(255, 48, 101, 172, vertices);
		}
		
		public void DeepCopy()
		{
			if(points != null)
				points = (PointF[])points.Clone();
			
			brush = new SolidBrush(brush.Color);
		}
		public void SetFriend(int index){}
	}
	
	public struct ColorCircle : ECS.IComponentData
	{
		public SolidBrush brush;
		public float radius;
		
		public ColorCircle ARGB(byte a, byte r, byte g, byte b){ brush.Color = Color.FromArgb(a,r,g,b); return this; }
		
		public static ColorCircle Create(byte a, byte r, byte g, byte b, float radius_)
		{
			return new ColorCircle
			{
				brush = new SolidBrush(Color.FromArgb(a,r,g,b)),
				radius = radius_
			};
		}
		
		public static ColorCircle Default()
		{
			return Create(255, 48, 101, 172, 0.5f);
		}
		
		public void DeepCopy()
		{
			brush = new SolidBrush(brush.Color);
		}
		
		public void SetFriend(int index){}
	}
}